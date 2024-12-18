using System;

namespace DataAccess.Entities.Administration
{
    public class PasswordResetToken
    {
        public int Id { get; set; }  // Clave primaria
        public string Email { get; set; }  // Almacenamos el correo
        public string Token { get; set; }
        public DateTime Expiration { get; set; }  // Fecha de expiración del token
        public bool IsUsed { get; set; }  // Indica si el token ya ha sido utilizado
    }
}
