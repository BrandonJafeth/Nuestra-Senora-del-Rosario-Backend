using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ApplicationFormUpdateDto
    {
        [Required]
        public string GuardianName { get; set; }

        [Required]
        public string GuardianLastName1 { get; set; }

        public string GuardianLastName2 { get; set; }

        [Required]
        public string GuardianCedula { get; set; }

        [Required]
        public string GuardianEmail { get; set; }

        [Required]
        public string GuardianPhone { get; set; }

        [Required]
        public string Name_AP { get; set; }

        [Required]
        public string LastName1_AP { get; set; }

        [Required]
        public string LastName2_AP { get; set; }

        [Required]
        public string Cedula_AP { get; set; }

        public int Age_AP { get; set; }

        public string Location_AP { get; set; }

        // Agrega más campos si deseas que se puedan actualizar
    }
}
