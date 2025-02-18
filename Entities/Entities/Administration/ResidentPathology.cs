using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class ResidentPathology
    {
        public int Id_ResidentPathology { get; set; }


        #region Atributes


        public string Resume_Pathology { get; set; }

        public DateOnly DiagnosisDate { get; set; }

        public DateOnly RegisterDate { get; set; }

        public string Notes { get; set; }

        #endregion



        #region Relations 
        public int Id_Resident { get; set; }
        public Resident Resident { get; set; }

        public int Id_Pathology { get; set; }
        public Pathology Pathology { get; set; }
        #endregion


        #region Audit
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        #endregion
    }
}
