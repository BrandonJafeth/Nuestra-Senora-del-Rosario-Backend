using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class ResidentApplication
    {
        [Key]
        public int Id_Relation { get; set; }

        [ForeignKey("Resident")]
        public int Id_Resident { get; set; }
        public Resident Resident { get; set; }  // Relación con Resident (Residente)

        [ForeignKey("Applicant")]
        public int Id_Applicant { get; set; }
        public Applicant Applicant { get; set; }  // Relación con Applicant (Solicitante)
    }
}
