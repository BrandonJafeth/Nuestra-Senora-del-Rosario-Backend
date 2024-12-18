using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.Administration
{
    public class PaymentReceipt
    {
        [Key]  // Clave primaria, identifica de manera única el comprobante de pago
        public int Id { get; set; }

        [Required]  // Obligatorio
        [ForeignKey("Employee")]  // Clave foránea que referencia a la entidad "Employee"
        public int EmployeeDni { get; set; }

        // Relación con la entidad Employee, indica el empleado relacionado con este comprobante
        public Employee Employee { get; set; }

        [Required]  // Obligatorio
        public DateTime PaymentDate { get; set; }  // Fecha de pago

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal Salary { get; set; }  // Salario base del empleado

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal Overtime { get; set; }  // Horas extras trabajadas

        // Nuevos campos agregados con sus atributos correspondientes
        [Required]  // Obligatorio
        public int WorkedDays { get; set; }  // Días trabajados

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal GrossIncome { get; set; }  // Ingreso bruto del empleado (antes de deducciones)

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal GrossAmount { get; set; }  // Monto bruto total (suma de salario, horas extras, etc.)

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal TotalExtraHoursAmount { get; set; }  // Monto total devengado por horas extras

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal ExtraHourRate { get; set; }  // Tasa por hora extra

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal DoubleExtras { get; set; }  // Horas extras dobles

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal NightHours { get; set; }  // Horas nocturnas trabajadas

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal Adjustments { get; set; }  // Ajustes aplicados al pago

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal Incapacity { get; set; }  // Monto relacionado con incapacidades

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal Absence { get; set; }  // Monto relacionado con ausencias

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal VacationDays { get; set; }  // Días de vacaciones tomados

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal NetAmount { get; set; }  // Monto neto a recibir (después de deducciones)

        [Required]  // Obligatorio
        [Column(TypeName = "decimal(18, 2)")]  // Define que el tipo de dato es decimal con 2 decimales
        public decimal TotalDeductions { get; set; }  // Suma total de las deducciones aplicadas

        [MaxLength(500)]  // Longitud máxima de 500 caracteres
        public string Notes { get; set; }  // Notas adicionales sobre el pago

        [Required]  // Obligatorio
        public DateTime CreatedAt { get; set; }  // Fecha de creación del comprobante de pago

        // Relación con deducciones
        public ICollection<Deduction> DeductionsList { get; set; }  // Lista de deducciones aplicadas al recibo de pago
    }
}
