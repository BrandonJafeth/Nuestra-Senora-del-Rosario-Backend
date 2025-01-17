using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Administration
{
    public class Resident
    {
        [Key]
        public int Id_Resident { get; set; }

        [Required, MaxLength(100)]
        public string Name_AP { get; set; }  // Nombre del residente

        [Required, MaxLength(100)]
        public string Lastname1_AP { get; set; }  // Primer apellido del residente

        [Required, MaxLength(100)]
        public string Lastname2_AP { get; set; }  // Segundo apellido del residente

        [Required, MaxLength(50)]
        public string Cedula_AP { get; set; }  // Cédula del residente

        [Required]
        public string Sexo { get; set; }  // Sexo (Femenino, Masculino)

        [Required]
        public DateTime FechaNacimiento { get; set; }  // Fecha de nacimiento

        [ForeignKey("Guardian")]
        public int Id_Guardian { get; set; }
        public Guardian Guardian { get; set; }  // Relación con Guardian

        [ForeignKey("Room")]
        public int Id_Room { get; set; }
        public Room Room { get; set; }  // Relación con Room (Habitación)

        [Required]
        public string Status { get; set; } = "Activo";  // Estado del residente (Activo, Inactivo)

        [Required]
        public DateTime EntryDate { get; set; }  // Fecha de ingreso



        [Required, MaxLength(250)]
        public string Location { get; set; }  // Localización del residente

        // Relación con el historial de dependencia
        public ICollection<DependencyHistory> DependencyHistories { get; set; }  // Historial de dependencia (colección de registros)

        public ICollection<Appointment> Appointments { get; set; }

        // Esta propiedad es opcional si deseas cargar el último nivel de dependencia
        [NotMapped]
        public DependencyHistory LatestDependencyHistory => DependencyHistories?.OrderByDescending(d => d.Id_History).FirstOrDefault();
    }
}
