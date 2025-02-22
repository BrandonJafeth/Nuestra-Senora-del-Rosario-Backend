using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class MedicationSpecificUpdateDto
    {

        public string Name_MedicamentSpecific { get; set; }
        public string SpecialInstructions { get; set; }
        public string AdministrationSchedule { get; set; }

        public int UnitOfMeasureID { get; set; }
        public int Id_AdministrationRoute { get; set; }

        // Si decides actualizar fecha de modificación
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }

}
