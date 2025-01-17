using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class AppointmentPostDto
    {
        [Required]
        public int Id_Resident { get; set; }  // ID del residente al que se le asigna la cita

        [Required]
        public DateTime Date { get; set; }  // Fecha de la cita

        [Required]
        public TimeSpan Time { get; set; }  // Hora de la cita (en hora local)

        [Required]
        public int Id_HC { get; set; }  // ID del centro de atención

        [Required]
        public int Id_Specialty { get; set; }  // ID de la especialidad

        [Required]
        public int Id_Companion { get; set; }  // ID del empleado acompañante

        public string Notes { get; set; }  // Notas opcionales para la cita
    }

}
