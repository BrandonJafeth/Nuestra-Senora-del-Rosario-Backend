using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Informative
{
    public class AssociatesSection
    {
        public int Id_AssociatesSection { get; set; }
        public string QuestionTitle_AS { get; set; }
        public string Image_AS_Url { get; set; }
        public string Description_AS { get; set; }

        public string? ContactText_AS { get; set; }
    }
}
