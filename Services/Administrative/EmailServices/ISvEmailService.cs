using System.IO;
using System.Threading.Tasks;

namespace Services.Administrative.EmailServices
{
    public interface ISvEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);  // Método existente

        // Nuevo método que incluye la posibilidad de adjuntar archivos
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, MemoryStream pdfAttachment, string pdfFileName);
    }
}
