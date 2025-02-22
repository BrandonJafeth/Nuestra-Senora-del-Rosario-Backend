using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentMedicationGetDto
    {
        public int Id_ResidentMedication { get; set; }

        public decimal PrescribedDose { get; set; }
        public string UnitOfMeasureName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string ResidentName { get; set; } // Ejemplo

        public string Name_MedicamentSpecific { get; set; } // Ejemplo
    }

}
