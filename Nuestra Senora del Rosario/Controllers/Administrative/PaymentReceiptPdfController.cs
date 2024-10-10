using Microsoft.AspNetCore.Mvc;
using Services.Administrative.PaymentReceiptService;
using Services.Administrative.PdfReceiver;
using Services.Administrative.PdfReceiverService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PaymentReceiptPdfController : ControllerBase
{
    private readonly ISvPdfReceiverService _pdfReceiverService;
    private readonly ISvPaymentReceipt _paymentReceiptService;

    public PaymentReceiptPdfController(ISvPdfReceiverService pdfReceiverService, ISvPaymentReceipt paymentReceiptService)
    {
        _pdfReceiverService = pdfReceiverService;
        _paymentReceiptService = paymentReceiptService;
    }

    // POST: api/PaymentReceiptPdf/send/{receiptId}
    [HttpPost("send/{receiptId}")]
    public async Task<IActionResult> SendPaymentReceiptPdf(int receiptId)
    {
        try
        {
            // Obtener el recibo de pago por ID
            var receipt = await _paymentReceiptService.GetPaymentReceiptByIdAsync(receiptId);

            if (receipt == null || string.IsNullOrEmpty(receipt.EmployeeEmail))
                return BadRequest("El recibo de pago no existe o el empleado no tiene correo electrónico.");

            // Generar el PDF del recibo de pago
            var pdfStream = await _paymentReceiptService.GeneratePaymentReceiptPdf(receipt);

            // Usar el servicio PdfReceiver para enviar el PDF por correo
            await _pdfReceiverService.ProcessReceivedPdfAsync(receipt.EmployeeEmail, pdfStream, "ComprobantePago.pdf");

            return Ok("Comprobante de pago enviado correctamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al enviar el comprobante de pago: {ex.Message}");
        }
    }
}
