namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentGetDto
    {
        public int Id_Resident { get; set; }
        public string Name_RD { get; set; }
        public string Lastname1_RD { get; set; }
        public string Lastname2_RD { get; set; }
        public string Cedula_RD { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string GuardianName { get; set; }  // Nombre completo del guardián
        public string GuardianPhone { get; set; }  // Teléfono del guardián (nuevo campo)
        public string RoomNumber { get; set; }  // Número de la habitación
        public string Status { get; set; }  // Activo o Inactivo
        public DateTime EntryDate { get; set; }
        public string DependencyLevel { get; set; }  // Nivel de dependencia
        public string Location_RD { get; set; }  // Localización del residente

        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;  // Si aún no ha cumplido este año, resta 1
                return age;
            }
        }
    }
}
