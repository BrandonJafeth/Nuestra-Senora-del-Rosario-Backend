using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class UnitOfMeasure
    {
        public int UnitOfMeasureID { get; set; }
        public string UnitName { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }
        public ICollection<MedicationSpecific> MedicationsSpecific { get; set; }
    }
}
