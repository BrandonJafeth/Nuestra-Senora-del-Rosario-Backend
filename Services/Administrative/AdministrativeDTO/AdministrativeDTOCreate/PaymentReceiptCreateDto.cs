using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;

public class PaymentReceiptCreateDto
{
    public int EmployeeDni { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Salary { get; set; }
    public decimal Overtime { get; set; }

    // Nuevos campos
    public int WorkedDays { get; set; }
    public decimal GrossIncome { get; set; }
    public decimal TotalExtraHoursAmount { get; set; } // Total por horas extras
    public decimal ExtraHourRate { get; set; } // Tasa por hora extra
    public decimal DoubleExtras { get; set; }  // Horas dobles
    public decimal NightHours { get; set; }    // Horas nocturnas
    public decimal Adjustments { get; set; }   // Ajustes
    public decimal Incapacity { get; set; }    // Incapacidades
    public decimal Absence { get; set; }       // Ausencias
    public decimal VacationDays { get; set; }  // Días de vacaciones

    public List<DeductionCreateDto> DeductionsList { get; set; }
    public decimal TotalDeductions { get; set; }
    public string Notes { get; set; }
}
