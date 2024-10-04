using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class PaymentReceiptCreateDto
    {
        public int EmployeeDni { get; set; }  // DNI del empleado
        public DateTime PaymentDate { get; set; }  // Fecha de pago
        public decimal Salary { get; set; }  // Salario base
        public decimal Overtime { get; set; }  // Horas extra
        public decimal GrossAmount { get; set; }  // Monto bruto
        public string Notes { get; set; }  // Notas adicionales

        // Lista de deducciones a aplicar en el comprobante
        public List<DeductionCreateDto> DeductionsList { get; set; }  
    }
}
