using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{

    public class UserGetDTO
    {
        public int Id_User { get; set; }  // Id del usuario
        public string EmployeeName { get; set; }  // Nombre del empleado
        public string RoleName { get; set; }  // Nombre del rol
        public bool Is_Active { get; set; }  // Estado activo/inactivo
    }
}
