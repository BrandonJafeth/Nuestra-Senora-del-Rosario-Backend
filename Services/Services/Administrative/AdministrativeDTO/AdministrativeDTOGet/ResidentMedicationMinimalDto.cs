using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentMedicationMinimalDto
    {
        public int Id_MedicamentSpecific { get; set; }
        public int Id_ResidentMedication { get; set; }
        public string Name_MedicamentSpecific { get; set; }
        public decimal PrescribedDose { get; set; }
        public string UnitOfMeasureName { get; set; }

        public string Notes { get; set; }
    }
}
