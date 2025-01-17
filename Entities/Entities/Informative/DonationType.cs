using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Informative
{
    public class DonationType
    {
        public int Id_DonationType { get; set; }
        public string Name_DonationType { get; set; }

        // Relación uno a muchos con FormDonations
        public ICollection<FormDonation> FormDonations { get; set; }
        public ICollection<MethodDonation> MethodDonations { get; set; }
    }
}
