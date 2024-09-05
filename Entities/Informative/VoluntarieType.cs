using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Informative
{
   public class VoluntarieType
    {
        public int Id_VoluntarieType { get; set; }
        public string Name_VoluntarieType { get; set; }
       
        public ICollection<FormVoluntarie> FormVoluntaries { get; set; }

    }
}
