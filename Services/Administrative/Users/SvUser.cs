using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.EmailServices;
using Services.GenericService;
using Services.Security;
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
        private readonly ISvEmailService _emailService; // Servicio de email inyectado
        private readonly IConfiguration _configuration; // Para acceder a las configuraciones de JWT

        public SvUser(IMapper mapper, ISvGenericRepository<User> userRepository,
                      ISvGenericRepository<Employee> employeeRepository,
                      ISvEmailService emailService, IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Método para crear un usuario desde un empleado (y generar contraseña aleatoria)
        public async Task CreateUserFromEmployeeAsync(int dniEmployee)
        {
            var employee = await _employeeRepository.GetByIdAsync(dniEmployee);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with DNI {dniEmployee} not found.");
            }

            var randomPassword = PasswordGenerator.GenerateRandomPassword(); // Implementa tu lógica de generación de contraseñas

            var user = new User
            {
                Dni_Employee = dniEmployee,
                Password = HashPassword(randomPassword), // Implementa el hashing de contraseña
                Is_Active = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Enviar correo con la contraseña aleatoria (usar el servicio de correo inyectado)
            await _emailService.SendEmailAsync(employee.Email, "New Account", $"Your temporary password is: {randomPassword}");
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

        // Método para hashear contraseñas (ejemplo simple, usa una librería segura en producción)
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Ejemplo con BCrypt
        }

        // Nuevo método para login y generación de token JWT
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            // Aquí aplicamos la solución para el error CS1501.
            var users = await _userRepository.GetAllAsync();  // Obtener todos los usuarios
            var user = users.FirstOrDefault(u => u.Dni_Employee == loginDTO.DniEmployee && u.Is_Active);  // Filtrar

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return null; // Retorna null si las credenciales no son válidas
            }

         

            // Crear los claims del token JWT, incluyendo roles si los hay
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Dni_Employee.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Dni_Employee.ToString())
            };

            // Añadir los roles al token como claims
         
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

            // Retornar el token generado
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
