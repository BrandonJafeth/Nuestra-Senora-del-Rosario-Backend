using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class EmployeeByRoleGetDto
    {
        public int Dni { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }
}
