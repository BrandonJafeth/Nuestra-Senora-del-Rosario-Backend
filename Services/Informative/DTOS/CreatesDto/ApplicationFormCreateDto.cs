using System;

namespace Services.Informative.DTOS.CreatesDto
{
    public class ApplicationFormCreateDto
    {
        // Datos del Applicant (nuevo o ya existente)

        public int Id_ApplicationForm { get; set; }
        public string Name_AP { get; set; }
        public string Lastname1_AP { get; set; }
        public string Lastname2_AP { get; set; }
        public int Age_AP { get; set; }
        public string Cedula_AP { get; set; }

         public string Location { get; set; }

        // Datos del Guardian (nuevo o ya existente)
        public string Name_GD { get; set; }
        public string Lastname1_GD { get; set; }
        public string Lastname2_GD { get; set; }
        public string Cedula_GD { get; set; }
        public string Phone_GD { get; set; }
        public string Email_GD { get; set; }
    }
}
