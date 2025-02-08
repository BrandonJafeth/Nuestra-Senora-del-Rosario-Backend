using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentFromApplicantDto
    {
        public int Id_ApplicationForm { get; set; } // ID del formulario de solicitud aprobado
        public int Id_Room { get; set; } // ID de la habitación asignada
        public DateTime EntryDate { get; set; } = DateTime.UtcNow; // Fecha de ingreso
        public string Sexo { get; set; } // Sexo del residente (Femenino o Masculino)
        public int Id_DependencyLevel { get; set; } // Nivel de dependencia asignado al residente
        public DateTime FechaNacimiento { get; set; } // Fecha de nacimiento del residente
    }

}
