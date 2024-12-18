using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationForm
{
    [Key]
    public int Id_ApplicationForm { get; set; }

    // Foreign key al solicitante
    [ForeignKey("Applicant")]
    public int Id_Applicant { get; set; }
    public Applicant Applicant { get; set; }    // Relación con Applicant

    // Foreign key al guardián
    [ForeignKey("Guardian")]
    public int Id_Guardian { get; set; }
    public Guardian Guardian { get; set; }      // Relación con Guardian

    [Required]
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;  // Fecha de creación

    // Foreign key al estado
    [ForeignKey("ApplicationStatus")]
    public int Id_Status { get; set; }
    public ApplicationStatus ApplicationStatus { get; set; }  // Relación con ApplicationStatus
}
