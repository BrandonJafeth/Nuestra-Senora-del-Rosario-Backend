using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationForm
{

    #region Atributes
    [Key]
    public int Id_ApplicationForm { get; set; }

    [Required]
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;  

    public string GuardianName { get; set; }
    public string GuardianLastName1 { get; set; }

    public string GuardianLastName2 { get; set; }

    public string GuardianCedula { get; set; }

    public string GuardianEmail { get; set; }

    public string GuardianPhone { get; set; }

    public string Name_AP { get; set; }

    public string LastName1_AP { get; set; }

    public string LastName2_AP { get; set; }

    public string Cedula_AP { get; set; }

    public int Age_AP { get; set; }

    public string Location_AP { get; set; }

    #endregion


    #region Relations
    // Foreign key al estado
    [ForeignKey("ApplicationStatus")]
    public int Id_Status { get; set; }
    public ApplicationStatus ApplicationStatus { get; set; }  // Relación con ApplicationStatus
    #endregion
}
