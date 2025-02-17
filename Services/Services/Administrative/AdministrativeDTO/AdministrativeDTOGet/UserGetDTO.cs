using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{

    public class UserGetDto
    {
        public int id_User { get; set; } // ID del usuario
        public int Dni { get; set; } // DNI del usuario
        public string Email { get; set; } // Nuevo campo
        public bool Is_Active { get; set; } // Estado del usuario
        public string FullName { get; set; } 
        public List<string> Roles { get; set; } // Roles asignados al usuario
    }

}
