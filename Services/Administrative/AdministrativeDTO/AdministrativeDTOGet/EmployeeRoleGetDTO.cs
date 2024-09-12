using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class EmployeeRoleGetDTO
    {
        public int Dni_Employee { get; set; }
        public int IdRole { get; set; }
        public string RoleName { get; set; }
    }
}
