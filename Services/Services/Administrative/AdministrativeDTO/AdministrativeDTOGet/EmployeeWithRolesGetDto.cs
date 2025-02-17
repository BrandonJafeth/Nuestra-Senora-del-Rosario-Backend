using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class EmployeeWithRolesGetDto
    {
        public int Dni { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
    }
}
