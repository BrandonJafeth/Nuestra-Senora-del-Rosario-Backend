using Infrastructure.Services.Administrative.EmailServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.GenericService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.PasswordResetServices
{
    public class SvPasswordResetService : ISvPasswordResetService
    {
        private readonly ISvGenericRepository<User> _userRepository; // Repositorio de User
        private readonly ISvEmailService _emailService;
        private readonly IConfiguration _configuration;

        public SvPasswordResetService(
            ISvGenericRepository<User> userRepository,
            ISvEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Solicitar el reseteo de contraseña enviando el correo con el token
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            // Verificar si el usuario con el correo proporcionado existe
            var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;  // No se encontró el usuario con el correo proporcionado
            }

            // Generar un token de recuperación de contraseña
            var token = GenerateResetToken(user.Id_User);

            // Crear el enlace de recuperación de contraseña
            var resetLink = $"{_configuration["App:FrontendUrl"]}/reset-password?userId={user.Id_User}&token={token}";

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
    <title>Restablecimiento de Contraseña</title>
</head>
<body>
    <div class='container'>
        <h1>Restablecimiento de Contraseña</h1>
        <p>
            Hemos recibido una solicitud para restablecer tu contraseña. Si realizaste esta solicitud,
            haz clic en el enlace de abajo para continuar:
        </p>
        <p>
            <a href='{resetLink}' target='_blank'>Restablecer Contraseña</a>
        </p>
        <p>Si no realizaste esta solicitud, puedes ignorar este correo.</p>
        <div class='footer'>
            <p>Este enlace expirará en 30 minutos. Por favor, no compartas este enlace con nadie.</p>
        </div>
    </div>
</body>
</html>";

            // Enviar el correo electrónico con el cuerpo HTML
            await _emailService.SendEmailAsync(user.Email, "Restablecimiento de Contraseña", body);

            return true;  // El correo se envió exitosamente
        }

        // Método para generar el token JWT de reseteo de contraseña
        private string GenerateResetToken(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", userId.ToString())
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
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch (SecurityTokenExpiredException)
            {
                return false; // Token expirado
            }
            catch
            {
                return false; // Error en la validación
            }
        }

        public async Task UpdatePasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("El token no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("La nueva contraseña no puede estar vacía.");
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

                // Extraer el DNI del usuario desde el token
                var dniClaim = principal.Claims.FirstOrDefault(c => c.Type == "id");
                if (dniClaim == null)
                {
                    throw new ArgumentException("El token no contiene un identificador válido.");
                }

                var dniEmployee = int.Parse(dniClaim.Value);

                // Verificar si el usuario existe
                var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.DNI == dniEmployee);
                if (user == null)
                {
                    throw new KeyNotFoundException("Usuario no encontrado.");
                }

                // Actualizar la contraseña del usuario
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordExpiration = null;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
            }
            catch (SecurityTokenExpiredException)
            {
                throw new ArgumentException("El token ha expirado.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error al actualizar la contraseña: " + ex.Message);
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
                // Validar el token
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken == null)
                {
                    throw new ArgumentException("Token inválido.");
                }

                // Extraer el ID del usuario
                var idClaim = principal.Claims.FirstOrDefault(c => c.Type == "id");
                if (idClaim == null)
                {
                    throw new ArgumentException("Token inválido, no contiene información de usuario.");
                }

                var userId = int.Parse(idClaim.Value);

                // Buscar al usuario
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("Usuario no encontrado.");
                }

                // Actualizar la contraseña
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordExpiration = null;


                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new ArgumentException("El token ha expirado.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error en el restablecimiento de contraseña: {ex.Message}");
            }



        }
    }
}
