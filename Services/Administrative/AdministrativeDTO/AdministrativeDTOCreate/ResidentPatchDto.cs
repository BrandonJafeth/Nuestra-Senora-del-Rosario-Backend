using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentPatchDto
    {
        public int? Id_Room { get; set; }
        public string? Status { get; set; }
        public int? Id_DependencyLevel { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

}
