using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class EmployeeCreateDTO
    {
        public int Dni { get; set; }
        public string First_Name { get; set; }
        public string Last_Name1 { get; set; }
        public string Last_Name2 { get; set; }
        public string Phone_Number { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Emergency_Phone { get; set; }
        public int Id_TypeOfSalary { get; set; }
        public int Id_Profession { get; set; }
    }

}


