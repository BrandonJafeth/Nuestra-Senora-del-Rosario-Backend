using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class PasswordResetRequestDTO
    {
        public string Email { get; set; }  // Puede ser el correo o el DNI del usuario
    }
}
