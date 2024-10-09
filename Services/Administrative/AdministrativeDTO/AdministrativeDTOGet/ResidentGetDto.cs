using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentGetDto
    {
        public int Id_Resident { get; set; }
        public string Name_AP { get; set; }
        public string Lastname1_AP { get; set; }
        public string Lastname2_AP { get; set; }
        public string Cedula_AP { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string GuardianName { get; set; }  // Nombre completo del guardián
        public string RoomNumber { get; set; }  // Número de la habitación
        public string Status { get; set; }  // Activo o Inactivo
        public DateTime EntryDate { get; set; }
        public string DependencyLevel { get; set; }  // Nivel de dependencia (nuevo campo)

        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;  // Si aún no ha cumplido este año, resta 1
                return age;
            }
        }
    }
}
