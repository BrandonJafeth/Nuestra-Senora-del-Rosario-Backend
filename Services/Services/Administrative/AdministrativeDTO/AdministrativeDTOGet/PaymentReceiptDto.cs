using System;
using System.Collections.Generic;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class PaymentReceiptDto
    {
        public int Id { get; set; }
        public int EmployeeDni { get; set; }  // Identificador del empleado (DNI)
        public string EmployeeFullName { get; set; }  // Nombre completo del empleado (generado combinando nombres y apellidos)
        public string EmployeeEmail { get; set; }  // Correo electrónico del empleado
        public string Profession { get; set; }  // Profesión del empleado
        public string SalaryType { get; set; }  // Tipo de salario (por ejemplo: mensual, quincenal)

        public DateTime PaymentDate { get; set; }  // Fecha de pago
        public decimal Salary { get; set; }  // Salario base
        public decimal Overtime { get; set; }  // Horas extra trabajadas
        public decimal TotalDeductions { get; set; }  // Suma de todas las deducciones individuales
        public decimal GrossAmount { get; set; }  // Total bruto devengado
        public decimal NetAmount { get; set; }  // Monto neto a recibir

        public string Notes { get; set; }  // Notas adicionales
        public DateTime CreatedAt { get; set; }  // Fecha de creación del comprobante

        // Lista de deducciones asociadas
        public List<DeductionDto> DeductionsList { get; set; }  // Lista de deducciones individuales

        // Nuevos campos solicitados desde el frontend:
        public int WorkedDays { get; set; }  // Días trabajados
        public decimal GrossIncome { get; set; }  // Salario bruto devengado (antes de deducciones)
        public decimal ExtrasHours { get; set; }  // Horas extras trabajadas
        public decimal ExtraHourRate { get; set; }  // Tasa de pago por hora extra
        public decimal TotalExtraHoursAmount { get; set; }  // Total devengado por horas extras
        public decimal DoublesHours { get; set; }  // Horas dobles trabajadas
        public decimal MandatoryHolidays { get; set; }  // Feriados de pago obligatorio
        public decimal DoubleExtras { get; set; }  // Horas extras dobles
        public decimal MixedHours { get; set; }  // Horas mixtas trabajadas
        public decimal NightHours { get; set; }  // Horas nocturnas trabajadas
        public decimal Adjustments { get; set; }  // Ajustes en el pago
        public decimal Incapacity { get; set; }  // Incapacidades reportadas
        public decimal Absence { get; set; }  // Días de ausencia
        public decimal VacationDays { get; set; }  // Días de vacaciones tomados
    }
}
