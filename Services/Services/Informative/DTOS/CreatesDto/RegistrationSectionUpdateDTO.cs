using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
    public class RegistrationSectionUpdateDTO
    {
        public string Registration_MoreInfoPrompt { get; set; }

        public string Registration_SupportMessage { get; set; }
        public string RegistrationImage_Url { get; set; }
    }
}
