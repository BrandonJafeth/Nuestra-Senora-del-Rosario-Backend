using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class EmployeeGetDTO
    {
        public int Dni { get; set; }
        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string EmergencyPhone { get; set; }
        public string ProfessionName { get; set; }  // Nombre de la profesión (mapeado desde la entidad)
        public string TypeOfSalaryName { get; set; }  // Nombre del tipo de salario (mapeado desde la entidad)
    }
}
