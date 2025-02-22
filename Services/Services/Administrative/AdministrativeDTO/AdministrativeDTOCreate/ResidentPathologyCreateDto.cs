using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentPathologyCreateDto
    {
        public string Resume_Pathology { get; set; }

        // El front enviaría "yyyy-MM-dd", lo parsearías a DateOnly
        public string DiagnosisDate { get; set; }
        public string RegisterDate { get; set; }

        public string Notes { get; set; }

        public int Id_Resident { get; set; }
        public int Id_Pathology { get; set; }
    }

}
