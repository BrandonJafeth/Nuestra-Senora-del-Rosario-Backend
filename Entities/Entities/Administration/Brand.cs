using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Brand
    {
        public int IdBrand { get; set; }
        public string BrandName { get; set; }

        // One-to-many: one brand can have many models
        public ICollection<Model> Models { get; set; }
    }
}
