using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class AppointmentUpdateDto
    {
        [Required]
        public int Id_Appointment { get; set; }  // ID de la cita

        public DateTime? Date { get; set; }  // Nueva fecha (opcional)
        public TimeSpan? Time { get; set; }  // Nueva hora (opcional)
        public int? Id_Companion { get; set; }  // Nuevo acompañante (opcional)
        public int? Id_StatusAP { get; set; }  // Nuevo estado de la cita (opcional)

        public string? Notes { get; set; }  // Notas adicionales (opcional)
    }

}
