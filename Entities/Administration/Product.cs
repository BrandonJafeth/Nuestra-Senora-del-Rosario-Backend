using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
   public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }        
        public int UnitOfMeasureID { get; set; }    
        public int TotalQuantity { get; set; }      

        // Navigation properties
        public Category Category { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public ICollection<Inventory> Inventories { get; set; }

    }
}
