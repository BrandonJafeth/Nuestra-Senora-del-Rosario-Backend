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
        private readonly IMemoryCache _cache;  // Inyección de IMemoryCache

        public SvUser(
            IMapper mapper,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<EmployeeRole> employeeRoleRepository,
            ISvEmailService emailService,
            IConfiguration configuration,
            IMemoryCache cache)  // Inyectamos IMemoryCache
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _emailService = emailService;
            _configuration = configuration;
            _cache = cache;  // Asignamos IMemoryCache
        }


        // Crear usuario desde un empleado con rol asignado
        public async Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole)
        {
            var employee = await _employeeRepository.GetByIdAsync(dniEmployee);
            if (employee == null) throw new KeyNotFoundException($"Employee with DNI {dniEmployee} not found.");

            var employeeRole = new EmployeeRole { Dni_Employee = dniEmployee, Id_Role = idRole };
            await _employeeRoleRepository.AddAsync(employeeRole);
            await _employeeRoleRepository.SaveChangesAsync();

            var randomPassword = PasswordGenerator.GenerateRandomPassword();
            var user = new User
            {
                Dni_Employee = dniEmployee,
                Password = HashPassword(randomPassword),
                Is_Active = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var changePasswordLink = GenerateChangePasswordLink(dniEmployee);
            await _emailService.SendEmailAsync(employee.Email, "New Account",
                $"Your temporary password is: {randomPassword}. Click [here]({changePasswordLink}) to change your password.");
        }

        // Generar el link para cambiar contraseña
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
            return $"{_configuration["App:FrontendUrl"]}/change-password?dni={dniEmployee}&token={tokenString}";
        }

        // Obtener usuario por ID
        public async Task<IEnumerable<UserGetDTO>> GetAllUsersAsync()
        {
            // Incluir las relaciones necesarias para Employee y EmployeeRole
            var users = await _userRepository.Query()
                .Include(u => u.Employee) // Incluir la relación con Employee
                .ThenInclude(e => e.EmployeeRoles) // Incluir la relación con EmployeeRoles
                .ThenInclude(er => er.Rol) // Incluir la relación con Rol
                .ToListAsync();

            // Mapear a DTO usando AutoMapper
            return _mapper.Map<IEnumerable<UserGetDTO>>(users);
        }

        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            // Incluir las relaciones necesarias para Employee y EmployeeRole
            var user = await _userRepository.Query()
                .Include(u => u.Employee) // Incluir la relación con Employee
                .ThenInclude(e => e.EmployeeRoles) // Incluir la relación con EmployeeRoles
                .ThenInclude(er => er.Rol) // Incluir la relación con Rol
                .FirstOrDefaultAsync(u => u.Id_User == id);

            // Si el usuario no existe, retorna null
            if (user == null)
                return null;

            // Mapear a DTO usando AutoMapper
            return _mapper.Map<UserGetDTO>(user);
        }


        // Método para hashear contraseñas
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            // Iniciar el cronómetro
            var stopwatch = Stopwatch.StartNew();

            // Obtener al usuario por DNI antes de cualquier operación
            var user = await _userRepository.GetByDniAsync(loginDTO.DniEmployee);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Verificar la contraseña antes de hacer operaciones costosas
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Intentamos obtener los roles desde la caché
            var cacheKey = $"UserRoles_{loginDTO.DniEmployee}";
            if (!_cache.TryGetValue(cacheKey, out List<string> employeeRoles))
            {
                employeeRoles = await _employeeRoleRepository
                    .Query()
                    .Include(er => er.Rol)
                    .Where(er => er.Dni_Employee == loginDTO.DniEmployee && er.Rol != null)
                    .Select(er => er.Rol.Name_Role)
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, employeeRoles, cacheEntryOptions);
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

            // Paramos el cronómetro
            stopwatch.Stop();

            // Logueamos el tiempo total en milisegundos
            Console.WriteLine($"Login completed in {stopwatch.ElapsedMilliseconds} ms");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}