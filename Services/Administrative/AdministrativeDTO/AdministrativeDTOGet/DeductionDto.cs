using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class DeductionDto
    {
        public int Id { get; set; }  // Identificador de la deducción
        public int PaymentReceiptId { get; set; }  // Identificador del comprobante de pago al que pertenece la deducción
        public string Type { get; set; }  // Tipo de deducción (por ejemplo: 'Impuestos', 'Seguro Social')
        public decimal Amount { get; set; }  // Monto de la deducción
    }

}
