using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class MedicationSpecificCreateDto
    {
        // Campos que envías al crear
        public string Name_MedicamentSpecific { get; set; }
        public string SpecialInstructions { get; set; }
        public string AdministrationSchedule { get; set; }

        public int UnitOfMeasureID { get; set; }
        public int Id_AdministrationRoute { get; set; }
    }

}
