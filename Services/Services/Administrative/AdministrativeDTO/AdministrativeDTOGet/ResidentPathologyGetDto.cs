using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ResidentPathologyGetDto
    {
        public int Id_ResidentPathology { get; set; }

        public string Resume_Pathology { get; set; }

        // Manejo de DateOnly -> se expone como string o como DateTime?
        // Aquí un ejemplo con string "yyyy-MM-dd"
        public string DiagnosisDate { get; set; }
        public string RegisterDate { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relaciones
        public int Id_Resident { get; set; }
        public string ResidentName { get; set; }

        public int Id_Pathology { get; set; }
        public string Name_Pathology { get; set; }
    }

}
