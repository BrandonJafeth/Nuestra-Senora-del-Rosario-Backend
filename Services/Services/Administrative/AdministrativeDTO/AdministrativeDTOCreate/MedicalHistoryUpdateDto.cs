using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class MedicalHistoryUpdateDto
    {
        [Required, MaxLength(1000)]
        public string Diagnosis { get; set; }

        [MaxLength(1000)]
        public string Treatment { get; set; }

        public string Observations { get; set; }
    }
}
