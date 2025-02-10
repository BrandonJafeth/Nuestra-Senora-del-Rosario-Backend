using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class EmployeeByProfessionDTO
    {
        public int Dni { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Profession { get; set; }
    }

}
