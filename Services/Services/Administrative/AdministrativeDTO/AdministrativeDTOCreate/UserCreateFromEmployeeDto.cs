using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class UserCreateFromEmployeeDto
    {
        public int DniEmployee { get; set; } // DNI del empleado asociado
        public string Password { get; set; } // Contraseña (opcional si es generada)
        public bool IsActive { get; set; } = true; // Estado del usuario
        public string FullName { get; set; }  
    }

}
