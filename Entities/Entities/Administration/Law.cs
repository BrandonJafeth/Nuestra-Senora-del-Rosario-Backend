using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Law
    {
        public int IdLaw { get; set; }
        public string LawName { get; set; }
        public string LawDescription { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}
