using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
   public class AssociatesSectionUpdateDTO
    {
        public string QuestionTitle_AS { get; set; }
        public string Image_AS_Url { get; set; }
        public string Description_AS { get; set; }

        public string ContactText_AS { get; set; }
    }
}
