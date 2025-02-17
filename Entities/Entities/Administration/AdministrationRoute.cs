using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class AdministrationRoute
    {
        public int Id_AdministrationRoute { get; set; }
        [Required]
        [MaxLength(100)]
        public string RouteName { get; set; }


        public ICollection<MedicationSpecific> MedicationSpecifics { get; set; }
    }
}
