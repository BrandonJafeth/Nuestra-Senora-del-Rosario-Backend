using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class MedicationSpecific
    {
        #region Atributes
        public int Id_MedicamentSpecific { get; set; }
        public string Name_MedicamentSpecific { get; set; }

        [MaxLength(1000)]
        public string SpecialInstructions { get; set; } // Detalles especiales o contraindicaciones

        [MaxLength(500)]
        public string AdministrationSchedule { get; set; } // Información sobre la frecuencia de toma
        #endregion

        #region Relations

        public int UnitOfMeasureID { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }

        public ICollection<ResidentMedication> ResidentMedications { get; set; }

        public int Id_AdministrationRoute { get; set; }

        public AdministrationRoute AdministrationRoute { get; set; }

        #endregion


    }
}
