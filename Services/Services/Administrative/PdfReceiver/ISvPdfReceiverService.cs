using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.PdfReceiver
{
    public interface ISvPdfReceiverService
    {

        Task ProcessReceivedPdfAsync(string employeeEmail, MemoryStream pdfStream, string pdfFileName);
    }
}
