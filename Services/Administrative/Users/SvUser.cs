using AutoMapper;
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

        public SvUser(
            IMapper mapper,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<EmployeeRole> employeeRoleRepository,
            ISvEmailService emailService,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _emailService = emailService;
            _configuration = configuration;
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
        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserGetDTO>(user);
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<UserGetDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserGetDTO>>(users);
        }

        // Método para hashear contraseñas
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        // Iniciar sesión y generar token JWT
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.GetByDniAsync(loginDTO.DniEmployee);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var employeeRoles = await _employeeRoleRepository
                .Query()
                .Include(er => er.Rol)
                .Where(er => er.Dni_Employee == loginDTO.DniEmployee && er.Rol != null)
                .Select(er => er.Rol.Name_Role)
                .ToListAsync();

            if (!employeeRoles.Any())
                Console.WriteLine($"No se encontraron roles para el usuario con DNI: {loginDTO.DniEmployee}");
            else
                employeeRoles.ForEach(role => Console.WriteLine($"Rol encontrado: {role} para usuario con DNI {loginDTO.DniEmployee}"));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginDTO.DniEmployee.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", loginDTO.DniEmployee.ToString())
            };

            // Agregar roles como claims
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
