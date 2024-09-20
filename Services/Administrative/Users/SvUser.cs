using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.EmailServices;
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
using System.Diagnostics;

namespace Services.Administrative.Users
{
    public class SvUser : ISvUser
    {
        private readonly IMapper _mapper;
        private readonly ISvGenericRepository<User> _userRepository;
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvGenericRepository<EmployeeRole> _employeeRoleRepository;
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public SvUser(
            IMapper mapper,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<EmployeeRole> employeeRoleRepository,
            ISvEmailService emailService,
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _emailService = emailService;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole)
        {
            var employee = await _employeeRepository.GetByIdAsync(dniEmployee);
            if (employee == null) throw new KeyNotFoundException($"Employee with DNI {dniEmployee} not found.");

            // Asignar rol al empleado
            var employeeRole = new EmployeeRole { Dni_Employee = dniEmployee, Id_Role = idRole };
            await _employeeRoleRepository.AddAsync(employeeRole);
            await _employeeRoleRepository.SaveChangesAsync();

            // Generar contraseña temporal
            var randomPassword = PasswordGenerator.GenerateRandomPassword();
            var user = new User
            {
                Dni_Employee = dniEmployee,
                Password = HashPassword(randomPassword),
                Is_Active = true,
                PasswordExpiration = DateTime.Now.AddDays(10)  // Contraseña válida por 10 días
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Enviar correo con el link para cambiar la contraseña
            var changePasswordLink = GenerateChangePasswordLink(dniEmployee);

            // Crear el cuerpo del correo con HTML y estilos
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
            Tu cuenta ha sido creada exitosamente. Tu contraseña temporal es: <strong>{randomPassword}</strong>.
        </p>
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

            // Enviar el correo electrónico
            await _emailService.SendEmailAsync(employee.Email, "Tu Cuenta Ha Sido Creada", body);
        }


        // Generar link para cambio de contraseña
        private string GenerateChangePasswordLink(int dniEmployee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, dniEmployee.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", dniEmployee.ToString())
                },
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return $"{_configuration["App:FrontendUrl"]}/cambio-contraseña?dni={dniEmployee}&token={tokenString}";
        }

        // Verificar expiración de la contraseña temporal en el login
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            var stopwatch = Stopwatch.StartNew();

            // Obtener al usuario por DNI
            var user = await _userRepository.GetByDniAsync(loginDTO.DniEmployee);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Verificar si la contraseña temporal ha expirado
            if (user.PasswordExpiration.HasValue && DateTime.Now > user.PasswordExpiration.Value)
            {
                throw new UnauthorizedAccessException("Tu contraseña temporal ha expirado. Debes cambiarla para continuar.");
            }

            // Verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Caching de roles y generación de JWT
            var cacheKey = $"UserRoles_{loginDTO.DniEmployee}";
            if (!_cache.TryGetValue(cacheKey, out List<string> employeeRoles))
            {
                employeeRoles = await _employeeRoleRepository.Query()
                    .Include(er => er.Rol)
                    .Where(er => er.Dni_Employee == loginDTO.DniEmployee && er.Rol != null)
                    .Select(er => er.Rol.Name_Role)
                    .ToListAsync();

                _cache.Set(cacheKey, employeeRoles, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            // Crear los claims del token JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginDTO.DniEmployee.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", loginDTO.DniEmployee.ToString())
            };
            employeeRoles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            stopwatch.Stop();
            Console.WriteLine($"Login completed in {stopwatch.ElapsedMilliseconds} ms");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Obtener usuarios con información adicional
        public async Task<IEnumerable<UserGetDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.Query()
                .Include(u => u.Employee)
                .ThenInclude(e => e.EmployeeRoles)
                .ThenInclude(er => er.Rol)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserGetDTO>>(users);
        }

        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.Query()
                .Include(u => u.Employee)
                .ThenInclude(e => e.EmployeeRoles)
                .ThenInclude(er => er.Rol)
                .FirstOrDefaultAsync(u => u.Id_User == id);

            return user == null ? null : _mapper.Map<UserGetDTO>(user);
        }

        // Método para hashear contraseñas
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
