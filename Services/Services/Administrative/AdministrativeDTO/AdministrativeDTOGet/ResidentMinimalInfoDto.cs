using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentMinimalInfoDto
    {
        public int Id_Resident { get; set; }
        public string Name_RD { get; set; }
        public string Lastname1_RD { get; set; }
        public string Lastname2_RD { get; set; }
        public string Cedula_RD { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
        public DateTime EntryDate { get; set; }
        public string Location_RD { get; set; }

        // Para medicamentos: colección de DTOs con información detallada
        public IEnumerable<ResidentMedicationMinimalDto> Medications { get; set; }

        // Para patologías: colección de DTOs con información detallada
        public IEnumerable<ResidentPathologyMinimalDto> Pathologies { get; set; }

        // Información mínima de las citas
        public IEnumerable<AppointmentMinimalDto> Appointments { get; set; }
    }
}
