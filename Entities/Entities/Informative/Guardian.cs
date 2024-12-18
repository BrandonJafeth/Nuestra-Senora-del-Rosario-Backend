using System.ComponentModel.DataAnnotations;

public class Guardian
{
    [Key]
    public int Id_Guardian { get; set; }

    [Required, MaxLength(50)]
    public string Name_GD { get; set; }         // Nombre del guardián

    [Required, MaxLength(50)]
    public string Lastname1_GD { get; set; }    // Primer apellido del guardián

    [Required, MaxLength(50)]
    public string Lastname2_GD { get; set; }    // Segundo apellido del guardián

    [Required, MaxLength(50)]
    public string Cedula_GD { get; set; }       // Cédula del guardián

    [Required, MaxLength(255)]
    public string Email_GD { get; set; }        // Email del guardián

    [Required, MaxLength(20)]
    public string Phone_GD { get; set; }        // Teléfono del guardián
}
