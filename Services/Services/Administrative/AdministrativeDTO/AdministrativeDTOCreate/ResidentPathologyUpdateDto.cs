using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentPathologyUpdateDto
    {
       
        public string Resume_Pathology { get; set; }
        public string DiagnosisDate { get; set; }
        public string RegisterDate { get; set; }
        public string Notes { get; set; }

        public int Id_Resident { get; set; }
        public int Id_Pathology { get; set; }

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }

}
