using System.ComponentModel.DataAnnotations;

public class ApplicationStatus
{
    [Key]
    public int Id_Status { get; set; }

    [Required, MaxLength(50)]
    public string Status_Name { get; set; }  // Ejemplo: 'Pendiente', 'Aprobado', 'Rechazado'
}
