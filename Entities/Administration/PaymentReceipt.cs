using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class PaymentReceipt
    {
        public int Id { get; set; }  // Clave primaria del comprobante de pago
        public int EmployeeDni { get; set; }  // Relaciona el comprobante con el DNI del empleado
        public Employee Employee { get; set; }  // Navegación a la entidad Employee

        public decimal Overtime { get; set; }
        public DateTime PaymentDate { get; set; }  // Fecha del pago
        public decimal Salary { get; set; }  // Salario base
        public decimal Extras { get; set; }  // Horas extras
        public decimal Deductions { get; set; }  // Deducciones totales
        public decimal NetAmount { get; set; }  // Monto neto
        public decimal TotalDeductions { get; set; }  // Monto total de deducciones
        public decimal GrossAmount { get; set; }  // Monto bruto devengado
        public string Notes { get; set; }  // Notas adicionales
        public string PdfFilePath { get; set; }  // Ruta del archivo PDF (si existe)

        public DateTime CreatedAt { get; set; }
        public ICollection<Deduction> DeductionsList { get; set; }  // Relación 1:N con Deduction
    }

}