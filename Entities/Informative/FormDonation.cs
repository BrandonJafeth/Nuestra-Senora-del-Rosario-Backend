using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Informative
{
    public class FormDonation
    {
        public int Id_FormDonation { get; set; }
        public string Dn_Name { get; set; }
        public string Dn_Lastname1 { get; set; }
        public string Dn_Lastname2 { get; set; }
        public int Dn_Cedula { get; set; }
        public string Dn_Phone { get; set; }
        public DateTime Delivery_date { get; set; }

        // Relación con DonationType
        public int Id_DonationType { get; set; }
        public DonationType DonationType { get; set; }

        // Relación con MethodDonation
        public int Id_MethodDonation { get; set; }
        public MethodDonation MethodDonation { get; set; }
    }
}
