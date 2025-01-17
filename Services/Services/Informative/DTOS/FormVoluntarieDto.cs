using System;

namespace Infrastructure.Services.Informative.DTOS
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

        // Nombre del tipo de voluntariado
        public string Name_voluntarieType { get; set; }

        // Nuevo campo para el estado
        public string Status_Name { get; set; }  // Nombre del estado, como "Pendiente", "Aprobado", etc.
    }
}
