using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Informative
{
    public class MethodDonation
    {
        public int Id_MethodDonation { get; set; }
        public string Name_MethodDonation { get; set; }

        // Relación con DonationType
        public int DonationTypeId { get; set; }
        public DonationType DonationType { get; set; }

        // Relación con FormDonations
        public ICollection<FormDonation> FormDonations { get; set; }
    }
}
