using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class AppointmentStatus
    {
        public int Id_StatusAP { get; set; } 
        public string Name_StatusAP { get; set; }  

        public ICollection<Appointment> Appointments { get; set; }
    }

}
