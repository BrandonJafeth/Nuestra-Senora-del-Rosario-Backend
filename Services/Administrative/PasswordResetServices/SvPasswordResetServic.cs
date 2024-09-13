using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Administrative.EmailServices;
using Services.GenericService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.PasswordResetServices
{
    public class SvPasswordResetService : ISvPasswordResetService
    {
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SvPasswordResetService(ISvGenericRepository<Employee> employeeRepository, ISvEmailService emailService, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Verificar si el empleado con el correo proporcionado existe
            var employee = await _employeeRepository.Query().FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                return false; 
            }

            // Generar un token de recuperación de contraseña
            var token = GenerateResetToken(employee.Dni);

            // Crear el enlace de recuperación de contraseña
            var resetLink = $"{_configuration["App:FrontendUrl"]}/reset-password?dni={employee.Dni}&token={token}";

            // Enviar el correo electrónico con el enlace de restablecimiento de contraseña
            await _emailService.SendEmailAsync(employee.Email, "Password Reset Request", $"Click [here]({resetLink}) to reset your password.");

            return true;
        }

        private string GenerateResetToken(int dniEmployee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, dniEmployee.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", dniEmployee.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
