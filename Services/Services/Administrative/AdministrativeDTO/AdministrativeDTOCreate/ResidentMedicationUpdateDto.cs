using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentMedicationUpdateDto
    {

        public decimal PrescribedDose { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }

        public int Id_Resident { get; set; }
        public int Id_MedicamentSpecific { get; set; }

        // Actualizamos la fecha de modificación
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
