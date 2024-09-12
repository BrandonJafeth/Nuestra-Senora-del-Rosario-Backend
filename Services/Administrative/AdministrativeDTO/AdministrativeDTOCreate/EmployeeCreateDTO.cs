using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class EmployeeCreateDTO
    {
        public int Dni { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string EmergencyPhone { get; set; }
        public int TypeOfSalaryId { get; set; }
        public int ProfessionId { get; set; }
    }

}


