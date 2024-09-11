using System;

namespace Services.Informative.DTOS.CreatesDto
{
    public class FormVoluntarieCreateDto
    {
        public string Vn_Name { get; set; }
        public string Vn_Lastname1 { get; set; }
        public string Vn_Lastname2 { get; set; }
        public int Vn_Cedula { get; set; }
        public string Vn_Phone { get; set; }
        public string Vn_Email { get; set; }
        public DateTime Delivery_Date { get; set; }
        public DateTime End_Date { get; set; }

        // Aquí solo necesitamos el Id del tipo de voluntariado
        public int VoluntarieTypeId { get; set; }
    }
}
