using Microsoft.AspNetCore.Mvc;
using Services.Administrative.PaymentReceiptService;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.EmailServices;

[ApiController]
[Route("api/[controller]")]
public class PaymentReceiptController : ControllerBase
{
    private readonly ISvPaymentReceipt _paymentReceiptService;
    private readonly ISvEmailService _emailService;

    public PaymentReceiptController(ISvPaymentReceipt paymentReceiptService, ISvEmailService emailService)
    {
        _paymentReceiptService = paymentReceiptService;
        _emailService = emailService;
    }

    // Endpoint para crear un PaymentReceipt
    [HttpPost]
    public async Task<IActionResult> CreatePaymentReceipt([FromBody] PaymentReceiptCreateDto dto)
    {
        var paymentReceiptDto = await _paymentReceiptService.CreatePaymentReceiptAsync(dto);
        if (paymentReceiptDto == null)
        {
            return BadRequest("Error al crear el recibo de pago.");
        }
        return Ok(paymentReceiptDto);
    }

    // Endpoint para obtener un PaymentReceipt por su ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentReceipt(int id)
    {
        var paymentReceipt = await _paymentReceiptService.GetPaymentReceiptByIdAsync(id);
        if (paymentReceipt == null)
        {
            return NotFound();
        }
        return Ok(paymentReceipt);
    }

    // Endpoint para enviar el recibo de pago por correo con el PDF adjunto
    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendPaymentReceiptByEmail(int id, [FromQuery] string email)
    {
        var paymentReceipt = await _paymentReceiptService.GetPaymentReceiptByIdAsync(id);
        if (paymentReceipt == null)
        {
            return NotFound("Recibo de pago no encontrado.");
        }

        // Generar el PDF en el backend
        var pdfData = await _paymentReceiptService.GeneratePaymentReceiptPdf(paymentReceipt);

        // Enviar el correo con el PDF adjunto
        await _emailService.SendEmailWithAttachmentAsync(email, "Recibo de Pago", "Adjunto encontrarás tu recibo de pago.", pdfData, "payment_receipt.pdf");

        return Ok("Recibo de pago enviado por correo.");
    }

    // Endpoint para descargar el PDF del PaymentReceipt
    [HttpGet("DownloadPaymentReceiptPdf/{id}")]
    public async Task<IActionResult> DownloadPaymentReceiptPdf(int id)
    {
        var receiptDto = await _paymentReceiptService.GetPaymentReceiptByIdAsync(id);
        if (receiptDto == null)
        {
            return NotFound();
        }

        var pdfStream = await _paymentReceiptService.GeneratePaymentReceiptPdf(receiptDto);

        return File(pdfStream, "application/pdf", "ComprobantePago.pdf");
    }
}