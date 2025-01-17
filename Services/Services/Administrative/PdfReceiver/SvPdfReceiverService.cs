using Infrastructure.Services.Administrative.EmailServices;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.PdfReceiver
{
    public class SvPdfReceiverService : ISvPdfReceiverService
    {
        private readonly ISvEmailService _emailService;

        public SvPdfReceiverService(ISvEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task ProcessReceivedPdfAsync(string employeeEmail, MemoryStream pdfStream, string pdfFileName)
        {
            // Asumimos que no se guarda el PDF en la base de datos. Se enviará directamente por correo.

            var emailBody = "Adjunto encontrará su comprobante de pago.";

            // Enviar el PDF adjunto por correo al empleado
            await _emailService.SendEmailWithAttachmentAsync(employeeEmail, "Comprobante de Pago", emailBody, pdfStream, pdfFileName);
        }
    }
}
