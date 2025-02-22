using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class MedicationSpecificGetDto
    {
        public int Id_MedicamentSpecific { get; set; }
        public string Name_MedicamentSpecific { get; set; }
        public string SpecialInstructions { get; set; }
        public string AdministrationSchedule { get; set; }

        public int UnitOfMeasureID { get; set; }
        public string UnitOfMeasureName { get; set; } // ejemplo: para mostrar la unidad

        public int Id_AdministrationRoute { get; set; }
        public string RouteName { get; set; } // ejemplo: para mostrar la vía de administración

        // Solo agrega CreatedAt/UpdatedAt si lo agregas al dominio
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
