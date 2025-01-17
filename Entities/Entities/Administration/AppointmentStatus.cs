using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class AppointmentStatus
    {
        public int Id_StatusAP { get; set; }  // Primary Key
        public string Name_StatusAP { get; set; }  // Estado de la cita (Pendiente, Completada, Cancelada)

        // Relación con citas
        public ICollection<Appointment> Appointments { get; set; }
    }

}
