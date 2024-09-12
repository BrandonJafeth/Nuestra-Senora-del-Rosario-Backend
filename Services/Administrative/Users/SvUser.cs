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

namespace Services.Administrative.Users
{
    public class SvUser : ISvUser
    {
        private readonly IMapper _mapper;
        private readonly ISvGenericRepository<User> _userRepository;
        private readonly ISvGenericRepository<Employee> _employeeRepository; // Necesario para obtener el empleado
        private readonly ISvGenericRepository<EmployeeRole> _employeeRoleRepository; // Repositorio para EmployeeRole
        private readonly ISvEmailService _emailService; // Servicio de email inyectado
        private readonly IConfiguration _configuration; // Para acceder a las configuraciones de JWT

        public SvUser(IMapper mapper, ISvGenericRepository<User> userRepository,
                      ISvGenericRepository<Employee> employeeRepository,
                      ISvGenericRepository<EmployeeRole> employeeRoleRepository,  // Inyección de EmployeeRoleRepository
                      ISvEmailService emailService, IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository; // Asignación del repositorio inyectado
            _emailService = emailService;
            _configuration = configuration;
        }

        // Método para crear un usuario desde un empleado (y generar contraseña aleatoria)
        public async Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole)
        {
            var employee = await _employeeRepository.GetByIdAsync(dniEmployee);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with DNI {dniEmployee} not found.");
            }

            // Asignar rol al empleado (usa el repositorio _employeeRoleRepository para manejar EmployeeRole)
            var employeeRole = new EmployeeRole
            {
                Dni_Employee = dniEmployee,
                Id_Role = idRole
            };

            await _employeeRoleRepository.AddAsync(employeeRole); // Guardar el nuevo rol asignado
            await _employeeRoleRepository.SaveChangesAsync();  // Guardar cambios en la base de datos

            // Generar contraseña aleatoria y crear usuario
            var randomPassword = PasswordGenerator.GenerateRandomPassword();
            var hashedPassword = HashPassword(randomPassword);

            var user = new User
            {
                Dni_Employee = dniEmployee,
                Password = hashedPassword,
                Is_Active = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Enviar correo con el link de cambio de contraseña
            string changePasswordLink = GenerateChangePasswordLink(user.Dni_Employee);
            await _emailService.SendEmailAsync(employee.Email, "New Account",
                $"Your temporary password is: {randomPassword}. Click [here]({changePasswordLink}) to change your password.");
        }

        // Método para generar el link de cambio de contraseña
        private string GenerateChangePasswordLink(int dniEmployee)
        {
            // Puedes usar algún token de seguridad aquí, por ejemplo, un JWT o GUID generado
            var token = Guid.NewGuid().ToString();
            var changePasswordLink = $"{_configuration["App:FrontendUrl"]}/change-password?dni={dniEmployee}&token={token}";

            // Podrías guardar el token temporal en la base de datos si necesitas validarlo

            return changePasswordLink;
        }

        // Método para obtener un usuario por ID
        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserGetDTO>(user);
        }

        // Método para obtener todos los usuarios
        public async Task<IEnumerable<UserGetDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserGetDTO>>(users);
        }

        
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); 
        }

        
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            var users = await _userRepository.GetAllAsync();  // Obtener todos los usuarios
            var user = users.FirstOrDefault(u => u.Dni_Employee == loginDTO.DniEmployee && u.Is_Active);  // Filtrar

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return null; // Retorna null si las credenciales no son válidas
            }

            // Obtener los roles del empleado
            var employeeRoles = await _employeeRoleRepository.GetAllAsync();
            var rolesForUser = employeeRoles
    .Where(er => er.Dni_Employee == user.Dni_Employee && er.Rol != null)
    .Select(er => er.Rol.Name_Role);

            // Crear claims del token JWT, incluyendo roles
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Dni_Employee.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Dni_Employee.ToString())
            };

            foreach (var role in rolesForUser)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Obtener la clave secreta para firmar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
