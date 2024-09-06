using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Applicant
{
    [Key]
    public int Id_Applicant { get; set; }

    [Required, MaxLength(50)]
    public string Name_AP { get; set; }         // Nombre del solicitante

    [Required, MaxLength(50)]
    public string Lastname1_AP { get; set; }    // Primer apellido del solicitante

    [Required, MaxLength(50)]
    public string Lastname2_AP { get; set; }    // Segundo apellido del solicitante

    [Required]
    public int Age_AP { get; set; }             // Edad del solicitante

    [Required, MaxLength(50)]
    public string Cedula_AP { get; set; }       // Cédula del solicitante

    // Relación con el Guardian
    [ForeignKey("Guardian")]
    public int Id_Guardian { get; set; }        // Foreign key al guardián
    public Guardian Guardian { get; set; }      // Referencia de navegación al guardián
}
