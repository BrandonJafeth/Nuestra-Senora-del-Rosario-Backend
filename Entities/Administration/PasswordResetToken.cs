using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class PasswordResetToken
    {
        public int Id { get; set; }  // Clave primaria
        public string Email { get; set; }  // Almacenamos el correo
        public string Token { get; set; }
        public DateTime Expiration { get; set; }  // Fecha de expiración del token
    }
}
