using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class ResidentMedication
    {
        #region Atributes
        public int Id_ResidentMedication { get; set; }

        public decimal PrescribedDose { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  
        public string Notes { get; set; }
        #endregion
        #region Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        #endregion

  

        #region Relations

        public int Id_Resident { get; set; }

        public Resident Resident { get; set; }

        public int Id_MedicamentSpecific { get; set; }

        public MedicationSpecific MedicationSpecific { get; set; }


        #endregion 



    }
}
