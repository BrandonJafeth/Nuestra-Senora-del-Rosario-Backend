using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class MedicalHistoryGetDto
    {
        public int Id_MedicalHistory { get; set; }
        public int Id_Resident { get; set; }
        public string ResidentFullName { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Observations { get; set; }
    }
}