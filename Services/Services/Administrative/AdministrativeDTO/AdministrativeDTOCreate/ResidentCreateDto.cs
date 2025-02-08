using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentCreateDto
    {
        public string Name_RD { get; set; }
        public string Lastname1_RD { get; set; }
        public string Lastname2_RD { get; set; }
        public string Cedula_RD { get; set; }
        public string Sexo { get; set; }  // Femenino o Masculino
        public DateTime FechaNacimiento { get; set; }
        public int Id_Guardian { get; set; }  // ID del guardián
        public int Id_Room { get; set; }  // ID de la habitación
        public DateTime EntryDate { get; set; }
        public int Id_DependencyLevel { get; set; }  // ID del nivel de dependencia

        // Nueva propiedad Location
        public string Location_RD { get; set; }  // Localización del residente
    }
}
