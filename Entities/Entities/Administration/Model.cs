using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Model
    {
        public int IdModel { get; set; }
        public string ModelName { get; set; }

        // Relationship to Brand
        public int IdBrand { get; set; }
        public Brand Brand { get; set; }
    }
}
