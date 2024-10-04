using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
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
        public decimal Overtime { get; set; }  // Horas extra
        public decimal Deductions { get; set; }  // Total de deducciones (suma de todas las deducciones)
        public decimal NetAmount { get; set; }  // Monto neto a recibir
        public decimal TotalDeductions { get; set; }  // Suma de todas las deducciones individuales
        public decimal GrossAmount { get; set; }  // Total bruto devengado
        public string Notes { get; set; }  // Notas adicionales
        public string PdfFilePath { get; set; }  // Nombre del archivo PDF (o ruta, dependiendo de cómo se gestione)
        public DateTime CreatedAt { get; set; }  // Fecha de creación del comprobante

        // Lista de deducciones asociadas
        public List<DeductionDto> DeductionsList { get; set; }  // Lista de deducciones individuales
    }

}
