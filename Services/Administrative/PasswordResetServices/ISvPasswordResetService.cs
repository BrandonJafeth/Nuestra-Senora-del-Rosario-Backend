using System.Threading.Tasks;

namespace Services.Administrative.PasswordResetServices
{
    public interface ISvPasswordResetService
    {
        Task<bool> RequestPasswordResetAsync(string email);

        // Validar si el token de restablecimiento de contraseña es válido
        Task<bool> ValidatePasswordResetTokenAsync(string token);

        // Actualizar la contraseña usando un token válido
        Task UpdatePasswordAsync(string token, string newPassword);
        Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword);
    }
}
