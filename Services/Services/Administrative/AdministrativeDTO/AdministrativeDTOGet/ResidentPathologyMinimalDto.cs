using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentPathologyMinimalDto
    {
        public int Id_ResidentPathology { get; set; }
        public int Id_Pathology { get; set; }
        public string Name_Pathology { get; set; }

        public string Resume_Pathology { get; set; }

        public string Notes { get; set; }
    }
}
