using System;

namespace Services.DTOS
{
    public class FormVoluntarieDto
    {
        public int Id_FormVoluntarie { get; set; }
        public string Vn_Name { get; set; }
        public string Vn_Lastname1 { get; set; }
        public string Vn_Lastname2 { get; set; }
        public int Vn_Cedula { get; set; }
        public string Vn_Phone { get; set; }
        public string Vn_Email { get; set; }
        public DateTime Delivery_Date { get; set; }
        public DateTime End_Date { get; set; }

        // En lugar de VoluntarieTypeDto, solo incluimos el nombre del tipo de voluntariado
        public string VoluntarieTypeName { get; set; }
    }
}
