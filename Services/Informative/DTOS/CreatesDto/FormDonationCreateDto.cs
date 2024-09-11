using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.DTOS.CreatesDto
{
    public class FormDonationCreateDto
    {
        public string Dn_Name { get; set; }
        public string Dn_Lastname1 { get; set; }
        public string Dn_Lastname2 { get; set; }
        public int Dn_Cedula { get; set; }

        public string Dn_Email { get; set; }
        public string Dn_Phone { get; set; }

        public DateTime Delivery_date { get; set; }
        public int Id_DonationType { get; set; }
        public int Id_MethodDonation { get; set; }
    }
}
