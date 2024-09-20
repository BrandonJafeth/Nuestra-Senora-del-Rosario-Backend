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

namespace Services.Administrative.PasswordResetServices
{
    public class SvPasswordResetService : ISvPasswordResetService
    {
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvGenericRepository<User> _userRepository; // Repositorio de User
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SvPasswordResetService(
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<User> userRepository, // Repositorio inyectado
            ISvEmailService emailService,
            IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Solicitar el reseteo de contraseña enviando el correo con el token
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Verificar si el empleado con el correo proporcionado existe
            var employee = await _employeeRepository.Query().FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                return false;  // No se encontró el empleado con el correo proporcionado
            }

            // Generar un token de recuperación de contraseña
            var token = GenerateResetToken(employee.Dni);

            // Crear el enlace de recuperación de contraseña
            var resetLink = $"{_configuration["App:FrontendUrl"]}/restablecer-contraseña?dni={employee.Dni}&token={token}";

            // Crear el cuerpo del correo electrónico con HTML estilizado
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
        <title>Solicitud de Restablecimiento de Contraseña</title>
    </head>
    <body>
        <div class='container'>
            <h1>Solicitud de Restablecimiento de Contraseña</h1>
            <p>
                Hemos recibido una solicitud para restablecer tu contraseña. Si realizaste esta solicitud, 
                haz clic en el enlace de abajo para restablecer tu contraseña:
            </p>
            <p>
                <a href='{resetLink}' target='_blank'>Restablecer tu contraseña</a>
            </p>
            <p>Si no solicitaste el restablecimiento de tu contraseña, puedes ignorar este correo.</p>
            <div class='footer'>
                <p>Este enlace expirará en 30 minutos. Por seguridad, no compartas este enlace con nadie.</p>
            </div>
        </div>
    </body>
    </html>";

            // Enviar el correo electrónico con el cuerpo HTML
            await _emailService.SendEmailAsync(employee.Email, "Solicitud de Restablecimiento de Contraseña", body);

            return true;  // El correo se envió exitosamente
        }





        // Método para generar el token JWT de reseteo de contraseña
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

        // Validar que el token JWT sea válido y no esté expirado
        public async Task<bool> ValidatePasswordResetTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Verifica si el token ha expirado
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Sin retraso de validación de expiración
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Verificar que el token es válido y no está comprometido
                if (validatedToken == null)
                {
                    return false;
                }

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false; // Token ha expirado
            }
            catch (Exception)
            {
                return false; // Cualquier otro error de validación
            }
        }



        // Resetear la contraseña del usuario validando el token
        public async Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                throw new ArgumentException("Las contraseñas no coinciden.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Decodificar y validar el token JWT
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Verifica si el token ha expirado
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Sin retraso de validación de expiración
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Verificar que el token es válido y no está comprometido
                if (validatedToken == null)
                {
                    throw new ArgumentException("Token inválido.");
                }

                // Extraer el DNI del empleado desde el token
                var dniClaim = principal.Claims.FirstOrDefault(c => c.Type == "id");
                if (dniClaim == null)
                {
                    throw new ArgumentException("Token inválido, no contiene DNI.");
                }

                var dniEmployee = int.Parse(dniClaim.Value);

                // Verificar si el empleado existe
                var employee = await _employeeRepository.GetByIdAsync(dniEmployee);
                if (employee == null)
                {
                    throw new ArgumentException("Empleado no encontrado.");
                }

                // Verificar si existe un usuario asociado con el empleado
                var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Dni_Employee == dniEmployee);
                if (user == null)
                {
                    throw new ArgumentException("Usuario no encontrado para este empleado.");
                }

                // Hashear la nueva contraseña
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Guardar cambios
                await _userRepository.SaveChangesAsync();

                return true; // Contraseña actualizada correctamente
            }
            catch (SecurityTokenExpiredException)
            {
                throw new ArgumentException("El token ha expirado.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Token inválido: " + ex.Message);
            }
        }

        public Task UpdatePasswordAsync(string token, string newPassword)
        {
            throw new NotImplementedException();
        }



    }
}
