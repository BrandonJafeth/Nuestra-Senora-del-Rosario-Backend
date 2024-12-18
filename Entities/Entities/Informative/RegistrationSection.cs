using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Informative
{
    public class RegistrationSection
    {
        public int Id_RegistrationSection { get; set; }
        public string Registration_MoreInfoPrompt { get; set; }

        public string Registration_SupportMessage { get; set; }
        public string RegistrationImage_Url { get; set; }
    }
}
