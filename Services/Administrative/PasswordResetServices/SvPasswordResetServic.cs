using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Administrative.EmailServices;
using Services.GenericService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities.Administration; // Referencia a la entidad PasswordResetToken

namespace Services.Administrative.PasswordResetServices
{
    public class SvPasswordResetService : ISvPasswordResetService
    {
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvGenericRepository<User> _userRepository;
        private readonly ISvGenericRepository<PasswordResetToken> _tokenRepository;
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SvPasswordResetService(
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<User> userRepository,
            ISvGenericRepository<PasswordResetToken> tokenRepository, // Repositorio de token
            ISvEmailService emailService,
            IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Solicitar el reseteo de contraseña y enviar el correo con el token
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var employee = await _employeeRepository.Query().FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null) return false;

            var token = GenerateResetToken();
            var expiration = DateTime.Now.AddMinutes(30);

            // Guardar el token en la base de datos
            var resetToken = new PasswordResetToken
            {
                Email = email,
                Token = token,
                Expiration = expiration,
                IsUsed = false  // Marcamos que el token aún no ha sido usado
            };
            await _tokenRepository.AddAsync(resetToken);
            await _tokenRepository.SaveChangesAsync();

            // Crear el enlace de recuperación de contraseña
            var resetLink = $"{_configuration["App:FrontendUrl"]}/restablecer-contraseña?dni={employee.Dni}&token={token}";

            // Crear el cuerpo del correo electrónico
            var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 0; font-size: 16px; color: #333; }}
                    .container {{ padding: 20px; max-width: 600px; margin: auto; background-color: #fff; border: 1px solid #ddd; border-radius: 8px; }}
                    h1 {{ color: #333; }}
                    p {{ font-size: 16px; line-height: 1.5; }}
                    a {{ color: #3498db; text-decoration: none; font-weight: bold; }}
                    .footer {{ margin-top: 20px; font-size: 12px; color: #999; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Solicitud de Restablecimiento de Contraseña</h1>
                    <p>Haz clic en el enlace de abajo para restablecer tu contraseña:</p>
                    <p><a href='{resetLink}' target='_blank'>Restablecer tu contraseña</a></p>
                    <p>Este enlace expirará en 30 minutos.</p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(employee.Email, "Solicitud de Restablecimiento de Contraseña", body);
            return true;
        }

        // Generar token de restablecimiento
        private string GenerateResetToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validar que el token sea válido, no haya expirado ni sido usado
        public async Task<bool> ValidatePasswordResetTokenAsync(string token)
        {
            var resetToken = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Token == token);

            // Verifica que el token existe, no ha expirado y no ha sido usado
            if (resetToken == null || resetToken.Expiration < DateTime.Now || resetToken.IsUsed)
                return false;

            return true;
        }

        // Resetear la contraseña validando el token
        public async Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword) throw new ArgumentException("Las contraseñas no coinciden.");

            var resetToken = await _tokenRepository.Query().FirstOrDefaultAsync(t => t.Token == token);
            if (resetToken == null) throw new ArgumentException("Token inválido o expirado.");

            // Verificar si el token ya fue usado
            if (resetToken.IsUsed || resetToken.Expiration < DateTime.Now)
                throw new ArgumentException("El token ha expirado o ya ha sido usado.");

            var employee = await _employeeRepository.Query().FirstOrDefaultAsync(e => e.Email == resetToken.Email);
            if (employee == null) throw new ArgumentException("Empleado no encontrado.");

            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Dni_Employee == employee.Dni);
            if (user == null) throw new ArgumentException("Usuario no encontrado.");

            // Actualizar la contraseña del usuario
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.SaveChangesAsync();

            // Marcar el token como usado para que no se pueda volver a usar
            resetToken.IsUsed = true;
            await _tokenRepository.SaveChangesAsync();

            return true;
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await ResetPasswordAsync(token, newPassword, newPassword);
        }
    }
}
