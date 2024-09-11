using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.EmailServices { 
    public interface ISvEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
}
