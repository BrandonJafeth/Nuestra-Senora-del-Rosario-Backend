using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.PaymentReceipt
{
    public interface ISvPaymentReceipt
    {
        Task<PaymentReceiptDto> CreatePaymentReceiptAsync(PaymentReceiptCreateDto paymentReceiptCreateDto);  // Crear nuevo comprobante
        Task<IEnumerable<PaymentReceiptDto>> GetPaymentReceiptsByEmployeeAsync(int employeeDni);  // Obtener todos los recibos de pago de un empleado
        Task<PaymentReceiptDto> GetPaymentReceiptByIdAsync(int id);  // Obtener recibo de pago por ID
        Task SendPaymentReceiptByEmailAsync(int receiptId);  // Enviar el comprobante de pago por correo electrónico

        Task<MemoryStream> GeneratePaymentReceiptPdf(PaymentReceiptDto paymentReceiptDto);
    }
}
