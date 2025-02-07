using System;

namespace Infrastructure.Services.Informative.DTOS
{
    public class ApplicationFormDto
    {
        public int Id_ApplicationForm { get; set; }
        public DateTime ApplicationDate { get; set; }

        // Guardian
        public string GuardianName { get; set; }
        public string GuardianLastName1 { get; set; }
        public string GuardianLastName2 { get; set; }
        public string GuardianCedula { get; set; }
        public string GuardianEmail { get; set; }
        public string GuardianPhone { get; set; }

        // Solicitante
        public string Name_AP { get; set; }
        public string LastName1_AP { get; set; }
        public string LastName2_AP { get; set; }
        public string Cedula_AP { get; set; }
        public int Age_AP { get; set; }
        public string Location_AP { get; set; }

        //  Solo si quieres devolver el nombre del estado
        public string Status_Name { get; set; }
    }
}
