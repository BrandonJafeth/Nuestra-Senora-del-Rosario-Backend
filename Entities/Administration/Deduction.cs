using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class Deduction
    {
        public int Id { get; set; }  // Clave primaria de la deducción
        public int PaymentReceiptId { get; set; }  // Relación con el comprobante de pago
        public PaymentReceipt PaymentReceipt { get; set; }  // Navegación al comprobante de pago

        public string Type { get; set; }  // Tipo de deducción (Ej: CCSS, Embargo, etc.)
        public decimal Amount { get; set; }  // Monto de la deducción
    }

}
