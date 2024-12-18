using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Informative
{
    public class VolunteeringSection
    {
        public int Id_VolunteeringSection { get; set; }
        public string Title_Card_VT { get; set; }
        public string Image_Card_VT_Url { get; set; }
        public string Description_Card_VT { get; set; }
    }
}
