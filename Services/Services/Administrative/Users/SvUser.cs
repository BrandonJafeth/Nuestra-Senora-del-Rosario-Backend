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
using Infrastructure.Services.Administrative.PasswordResetServices;
using FluentValidation;

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
        private readonly ISvPasswordResetService _passwordResetService;
        private readonly IValidator<UserUpdateProfileDto> _updateProfileValidator;
        private readonly IValidator<UserChangePasswordDto> _changePasswordValidator;
        private readonly IValidator<UserStatusUpdateDto> _statusUpdateValidator;
        private readonly IValidator<UserCreateDto> _userCreateValidator;
        private readonly IValidator<(int dniEmployee, int idRole)> _userCreateFromEmployeeValidator;


        public SvUser(
            IMapper mapper,
             AppDbContext context,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<UserRoles> userRoleRepository,
             ISvGenericRepository<Employee> employeeRepository,
            ISvEmailService emailService,
            IConfiguration configuration,
            ISvPasswordResetService passwordResetService, IValidator<UserUpdateProfileDto> updateProfileValidator,
        IValidator<UserChangePasswordDto> changePasswordValidator,
        IValidator<UserStatusUpdateDto> statusUpdateValidator, IValidator<UserCreateDto> userCreateValidator,
    IValidator<(int dniEmployee, int idRole)> userCreateFromEmployeeValidator)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _employeeRepository = employeeRepository;
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _passwordResetService = passwordResetService;
            _updateProfileValidator = updateProfileValidator;
            _changePasswordValidator = changePasswordValidator;
            _statusUpdateValidator = statusUpdateValidator;
            _userCreateValidator = userCreateValidator;
            _userCreateFromEmployeeValidator = userCreateFromEmployeeValidator;
        }

        public async Task CreateUserAsync(UserCreateDto userCreateDto)
        {
            var validationResult = await _userCreateValidator.ValidateAsync(userCreateDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = _mapper.Map<User>(userCreateDto);
            string tempPassword = PasswordGenerator.GenerateRandomPassword();

            user.Password = HashPassword(tempPassword);
            user.Is_Active = true;
            user.PasswordExpiration = DateTime.Now.AddDays(10);
            user.FullName = userCreateDto.FullName;

            // Guardar el usuario
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var userRole = new UserRoles
            {
                Id_User = user.Id_User,
                Id_Role = userCreateDto.Id_Role
            };
            await _userRoleRepository.AddAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();

            await SendUserCredentialsEmailAsync(user.DNI, user.Email, tempPassword, user.FullName);
        }



        public async Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole)
        {
            // 🔹 Validación sin FluentValidation
            if (await ExistsUserByDniAsync(dniEmployee))
            {
                throw new InvalidOperationException($"El empleado con DNI {dniEmployee} ya tiene un usuario asociado.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var employee = await _employeeRepository.Query()
                    .FirstOrDefaultAsync(e => e.Dni == dniEmployee);

                if (employee == null)
                {
                    throw new KeyNotFoundException($"No se encontró el empleado con DNI {dniEmployee}.");
                }

                var randomPassword = PasswordGenerator.GenerateRandomPassword();
                string fullName = $"{employee.First_Name} {employee.Last_Name1} {employee.Last_Name2}".Trim();

                var user = new User
                {
                    DNI = dniEmployee,
                    Email = employee.Email,
                    Password = HashPassword(randomPassword),
                    Is_Active = true,
                    PasswordExpiration = DateTime.Now.AddDays(10),
                    FullName = fullName
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                var userRole = new UserRoles
                {
                    Id_User = user.Id_User,
                    Id_Role = idRole
                };

                await _userRoleRepository.AddAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                await SendUserCredentialsEmailAsync(user.DNI, user.Email, randomPassword, user.FullName);
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

        // Generar JWT con la lista de roles del usuario
        private string GenerateJwtToken(User user)
        {
            // Obtener la lista de roles asignados al usuario
            var userRoles = _userRoleRepository.Query()
                .Include(ur => ur.Role)
                .Where(ur => ur.Id_User == user.Id_User)
                .Select(ur => ur.Role.Name_Role)
                .ToList();

            if (userRoles == null || !userRoles.Any())
            {
                throw new ApplicationException("El usuario no tiene un rol asignado.");
            }

            // Crear los claims básicos
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id_User.ToString()), // Identificador del usuario
        new Claim("dni", user.DNI.ToString()) // DNI del usuario
    };

            // Añadir cada rol como un claim individual
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generar la clave y las credenciales para el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear el token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],      // Emisor
                audience: _configuration["Jwt:Audience"],  // Audiencia
                claims: claims,
                expires: DateTime.Now.AddHours(1),         // Expiración en 1 hora
                signingCredentials: creds                  // Credenciales de firma
            );

            // Retornar el token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // Enviar correo con credenciales
        private async Task SendUserCredentialsEmailAsync(int dni, string email, string password, string fullName)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El email no puede estar vacío.");
            }

            // Obtener el usuario por DNI antes de generar el token
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == dni);
            if (user == null)
            {
                throw new ArgumentException("Usuario no encontrado al generar el token de restablecimiento.");
            }

            
            var token = _passwordResetService.GenerateResetToken(user.Id_User);

            // 🔹 Usar el token en el enlace de cambio de contraseña
            var changePasswordLink = $"{_configuration["App:FrontendUrl"]}/reset-password?token={token}";

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
        <h1>¡Hola {fullName}!</h1>
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


        public async Task<UserGetMeDto> GetAuthenticatedUserAsync(int userId)
        {
            var user = await _userRepository.Query()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id_User == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            return new UserGetMeDto
            {
                Id_User = user.Id_User,
                DNI = user.DNI.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.Is_Active,
                Roles = user.UserRoles.Select(ur => ur.Role.Name_Role).ToList() 
            };
        }



        public async Task UpdateUserProfileAsync(int userId, UserUpdateProfileDto userUpdateProfileDto)
        {
            var validationResult = await _updateProfileValidator.ValidateAsync(userUpdateProfileDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id_User == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            user.FullName = userUpdateProfileDto.FullName;
            user.Email = userUpdateProfileDto.Email;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ChangeAuthenticatedUserPasswordAsync(int userId, UserChangePasswordDto userChangePasswordDto)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(userChangePasswordDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id_User == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userChangePasswordDto.CurrentPassword, user.Password))
            {
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(userChangePasswordDto.NewPassword);
            user.PasswordExpiration = null;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserStatusAsync(int userId, bool isActive)
        {
            var validationResult = await _statusUpdateValidator.ValidateAsync(new UserStatusUpdateDto { IsActive = isActive });
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Id_User == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            user.Is_Active = isActive;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }


        // Método para hashear contraseñas
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        private async Task<bool> ExistsUserByDniAsync(int dni)
        {
            return await _userRepository.Query().AnyAsync(u => u.DNI == dni);
        }

    }
}
