namespace Services.DTOS
{
    public class FormDonationDto
    {
        public int Id_FormDonation { get; set; }
        public string Dn_Name { get; set; }
        public string Dn_Lastname1 { get; set; }
        public string Dn_Lastname2 { get; set; }
        public int Dn_Cedula { get; set; }
        public string Dn_Phone { get; set; }
        public DateTime Delivery_date { get; set; }

        // Relación con DonationType
        public DonationTypeDto DonationType { get; set; }

        // Relación con MethodDonation
        public MethodDonationDto MethodDonation { get; set; }
    }
}
