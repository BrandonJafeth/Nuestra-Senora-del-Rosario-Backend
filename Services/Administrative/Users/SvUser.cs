using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using Services.Security;
using BCrypt.Net;
using Services.Administrative.EmailServices;

namespace Services.Administrative.Users
{
    public class SvUser : ISvUser
    {
        private readonly IMapper _mapper;
        private readonly ISvGenericRepository<User> _userRepository;
        private readonly ISvGenericRepository<Employee> _employeeRepository; // Necesario para obtener el empleado
        private readonly ISvEmailService _emailService; // Servicio de email inyectado

        public SvUser(IMapper mapper, ISvGenericRepository<User> userRepository,
                      ISvGenericRepository<Employee> employeeRepository, ISvEmailService emailService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;
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
    }
}
