using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Services.Administrative.EmailServices
{
    public class SvEmailService : ISvEmailService
    {
        private readonly IConfiguration _configuration;

        public SvEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Método original que envía un correo sin adjuntos
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);
            await client.SendMailAsync(mailMessage);
        }

        // Nuevo método que envía un correo con un archivo PDF adjunto
        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, MemoryStream pdfAttachment, string pdfFileName)
        {
            using var client = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            if (pdfAttachment != null)
            {
                // Reiniciar el stream a la posición inicial
                pdfAttachment.Position = 0;
                mailMessage.Attachments.Add(new Attachment(pdfAttachment, pdfFileName, "application/pdf"));
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}
