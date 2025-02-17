using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.PasswordResetServices
{
    public interface ISvPasswordResetService
    {
        /// <summary>
        /// Solicita el restablecimiento de contraseña enviando un correo electrónico al usuario.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <returns>Verdadero si se envió el correo, falso si el usuario no fue encontrado.</returns>
        Task<bool> RequestPasswordResetAsync(string email);

        /// <summary>
        /// Valida si un token de restablecimiento de contraseña es válido y no ha expirado.
        /// </summary>
        /// <param name="token">Token JWT para restablecer la contraseña.</param>
        /// <returns>Verdadero si el token es válido, falso en caso contrario.</returns>
        Task<bool> ValidatePasswordResetTokenAsync(string token);

        /// <summary>
        /// Actualiza la contraseña de un usuario usando un token válido.
        /// </summary>
        /// <param name="token">Token JWT válido.</param>
        /// <param name="newPassword">Nueva contraseña del usuario.</param>
        Task UpdatePasswordAsync(string token, string newPassword);

        /// <summary>
        /// Restablece la contraseña del usuario validando el token proporcionado.
        /// </summary>
        /// <param name="token">Token JWT para validar la operación.</param>
        /// <param name="newPassword">Nueva contraseña.</param>
        /// <param name="confirmPassword">Confirmación de la nueva contraseña.</param>
        /// <returns>Verdadero si la contraseña fue restablecida exitosamente.</returns>
        Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword);

        string GenerateResetToken(int userId);
    }
}
