using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Informative
{
    public class DonationsSection
    {
        public int Id_DonationsSection { get; set; }
        public string Description_Donations { get; set; }
        public string Donations_MoreInfoPrompt { get; set; }
        
    }
}
