using System;

namespace Services.Informative.DTOS
{
    public class ApplicationFormDto
    {
        public int Id_ApplicationForm { get; set; }



        // Datos del Aplicante

        public int Id_Applicant { get; set; }
        public string Name_AP { get; set; }
        public string Lastname1_AP { get; set; }
        public string Lastname2_AP { get; set; }
        public int Age_AP { get; set; }
        public string Cedula_AP { get; set; }

        // Datos del Guardian

        public int Id_Guardian { get; set; }
        public string Name_GD { get; set; }
        public string Lastname1_GD { get; set; }
        public string Lastname2_GD { get; set; }
        public string Cedula_GD { get; set; }
        public string Phone_GD { get; set; }
        public string Email_GD { get; set; }

        // Fecha de la solicitud
        public DateTime ApplicationDate { get; set; }

        // Nombre del estado (mapeado desde ApplicationStatus)
        public string Status_Name { get; set; }
    }
}
