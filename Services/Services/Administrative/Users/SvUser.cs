using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.GenericService;
using Services.Security;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Administrative.EmailServices;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Persistence.AppDbContext;

namespace Infrastructure.Services.Administrative.Users
{
    public class SvUser : ISvUser
    {
        private readonly IMapper _mapper;
        private readonly ISvGenericRepository<User> _userRepository;
        private readonly ISvGenericRepository<UserRoles> _userRoleRepository;
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly AppDbContext _context;

        public SvUser(
            IMapper mapper,
             AppDbContext context,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<UserRoles> userRoleRepository,
             ISvGenericRepository<Employee> employeeRepository,
            ISvEmailService emailService,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _employeeRepository = employeeRepository;
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Crear un usuario desde cero
        public async Task CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = _mapper.Map<User>(userCreateDto);

            // Generar la contraseña temporal
            string tempPassword = PasswordGenerator.GenerateRandomPassword();

            // Hashear la contraseña antes de guardarla en la base de datos
            user.Password = HashPassword(tempPassword);
            user.Is_Active = true;
            user.PasswordExpiration = DateTime.Now.AddDays(10);

            // Guardar el usuario en la base de datos
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Enviar la contraseña original al correo
            await SendUserCredentialsEmailAsync(user.DNI, user.Email, tempPassword);
        }


        public async Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                // Buscar el empleado en la base de datos
                var employee = await _employeeRepository.Query()
                    .FirstOrDefaultAsync(e => e.Dni == dniEmployee);

                if (employee == null)
                {
                    throw new KeyNotFoundException($"No se encontró el empleado con DNI {dniEmployee}.");
                }

                // Verificar si el usuario ya existe
                var existingUser = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == dniEmployee);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"El usuario con DNI {dniEmployee} ya existe.");
                }

                // Generar una contraseña aleatoria
                var randomPassword = PasswordGenerator.GenerateRandomPassword();

                // Crear el usuario con la información del empleado
                var user = new User
                {
                    DNI = dniEmployee,
                    Email = employee.Email,  // ✅ Se usa el email del empleado
                    Password = HashPassword(randomPassword),
                    Is_Active = true,
                    PasswordExpiration = DateTime.Now.AddDays(10) // La contraseña expira en 10 días
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Asignar el rol al usuario
                var userRole = new UserRoles
                {
                    Id_User = user.Id_User,
                    Id_Role = idRole
                };

                await _userRoleRepository.AddAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                // Enviar correo con credenciales
                await SendUserCredentialsEmailAsync(user.DNI, user.Email, randomPassword);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Ocurrió un error al crear el usuario desde el empleado.", ex);
            }
        }


        // Obtener un usuario por ID
        public async Task<UserGetDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.Query()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id_User == id);

            if (user == null) throw new KeyNotFoundException($"Usuario con ID {id} no encontrado.");

            return _mapper.Map<UserGetDto>(user);
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.Query()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserGetDto>>(users);
        }

        // Asignar un rol a un usuario
        public async Task AssignRoleToUserAsync(UserRoleCreateDTO userRoleCreateDto)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id_User == userRoleCreateDto.Id_User);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {userRoleCreateDto.Id_User} no encontrado.");
            }

            var userRole = new UserRoles
            {
                Id_User = userRoleCreateDto.Id_User,
                Id_Role = userRoleCreateDto.Id_Role
            };

            await _userRoleRepository.AddAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();
        }

        // Cambiar la contraseña del usuario
        public async Task ChangePasswordAsync(int dni, string newPassword)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == dni);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con DNI {dni} no encontrado.");
            }

            user.Password = HashPassword(newPassword);
            user.PasswordExpiration = DateTime.Now.AddDays(10);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        // Verificar si la contraseña ha expirado
        public async Task<bool> IsPasswordExpiredAsync(int dni)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == dni);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con DNI {dni} no encontrado.");
            }

            return user.PasswordExpiration.HasValue && DateTime.Now > user.PasswordExpiration.Value;
        }

        // Login
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == loginDTO.DniEmployee);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            if (user.PasswordExpiration.HasValue && DateTime.Now > user.PasswordExpiration.Value)
            {
                throw new UnauthorizedAccessException("Tu contraseña ha expirado. Debes cambiarla.");
            }

            return GenerateJwtToken(user);
        }

        // Generar JWT con el rol del usuario
        private string GenerateJwtToken(User user)
        {
            // Obtener el rol del usuario desde la relación UserRoles
            var userRole = _userRoleRepository.Query()
                .Include(ur => ur.Role)
                .Where(ur => ur.Id_User == user.Id_User)
                .Select(ur => ur.Role.Name_Role)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(userRole))
            {
                throw new ApplicationException("El usuario no tiene un rol asignado.");
            }

            // Crear los claims del token
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id_User.ToString()), // Identificador del usuario
        new Claim("dni", user.DNI.ToString()), // DNI del usuario
        new Claim(ClaimTypes.Role, userRole) // Rol del usuario
    };

            // Generar la clave y las credenciales para el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear el token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Emisor
                audience: _configuration["Jwt:Audience"], // Audiencia
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Expiración en 1 hora
                signingCredentials: creds // Credenciales de firma
            );

            // Retornar el token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Enviar correo con credenciales
        private async Task SendUserCredentialsEmailAsync(int dni, string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El email no puede estar vacío.");
            }

            var changePasswordLink = $"{_configuration["App:FrontendUrl"]}/cambio-contraseña?dni={dni}";

            var body = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            font-size: 16px;
            color: #333;
        }}
        .container {{
            padding: 20px;
            max-width: 600px;
            margin: auto;
            background-color: #fff;
            border: 1px solid #ddd;
            border-radius: 8px;
        }}
        h1 {{
            color: #333;
        }}
        p {{
            font-size: 16px;
            line-height: 1.5;
        }}
        a {{
            color: #3498db;
            text-decoration: none;
            font-weight: bold;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #999;
        }}
    </style>
    <title>Tu Cuenta Ha Sido Creada</title>
</head>
<body>
    <div class='container'>
        <h1>¡Bienvenido!</h1>
        <p>
            Tu cuenta ha sido creada exitosamente. Tus credenciales son:
        </p>
        <p><strong>DNI:</strong> {dni}</p>
        <p><strong>Contraseña temporal:</strong> {password}</p>
        <p>
            Por favor, haz clic en el siguiente enlace para cambiar tu contraseña:
        </p>
        <p>
            <a href='{changePasswordLink}' target='_blank'>Cambiar Contraseña</a>
        </p>
        <p>Este enlace es válido por 60 minutos. Después de este tiempo, necesitarás solicitar uno nuevo.</p>
        <div class='footer'>
            <p>Si no solicitaste la creación de esta cuenta, puedes ignorar este correo.</p>
        </div>
    </div>
</body>
</html>";

            try
            {
                await _emailService.SendEmailAsync(email, "Credenciales de Acceso", body);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al enviar el correo a {email}: {ex.Message}");
            }
        }


        // Método para hashear contraseñas
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
