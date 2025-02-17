namespace Infrastructure.Services.Informative.DTOS
{
    public class FormDonationDto
    {
        public int Id_FormDonation { get; set; }
        public string Dn_Name { get; set; }
        public string Dn_Lastname1 { get; set; }
        public string Dn_Lastname2 { get; set; }
        public int Dn_Cedula { get; set; }

        public string Dn_Email { get; set; }
        public string Dn_Phone { get; set; }
        public DateTime Delivery_date { get; set; }

        // Solo nombres de DonationType, MethodDonation, y el Estado
        public string DonationType { get; set; }
        public string MethodDonation { get; set; }
        public string Status_Name { get; set; }  // Nombre del estado, como "Pendiente", "Aprobado", etc.
    }
}
