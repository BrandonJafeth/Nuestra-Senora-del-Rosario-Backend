using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ResidentCreateDto
    {
        public string Name_AP { get; set; }
        public string Lastname1_AP { get; set; }
        public string Lastname2_AP { get; set; }
        public string Cedula_AP { get; set; }
        public string Sexo { get; set; }  // Femenino o Masculino
        public DateTime FechaNacimiento { get; set; }
        public int Id_Guardian { get; set; }  // ID del guardián
        public int Id_Room { get; set; }  // ID de la habitación
        public DateTime EntryDate { get; set; }
        public int Id_DependencyLevel { get; set; }  // ID del nivel de dependencia

        // Nueva propiedad Location
        public string Location { get; set; }  // Localización del residente
    }
}
