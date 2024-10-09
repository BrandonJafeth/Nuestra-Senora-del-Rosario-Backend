using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentFromApplicantDto
    {
        public int Id_Applicant { get; set; }  // ID del Applicant aprobado
        public int Id_Room { get; set; }  // ID de la habitación asignada al residente
        public DateTime EntryDate { get; set; }  // Fecha de ingreso del residente
        public string Sexo { get; set; }  // Sexo del residente (Femenino o Masculino)
        public int Id_DependencyLevel { get; set; }  // ID del nivel de dependencia asignado al residente
    }

}
