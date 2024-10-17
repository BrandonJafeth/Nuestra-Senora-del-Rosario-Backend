using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class Appointment
    {
        public int Id_Appointment { get; set; }
        public int Id_Resident { get; set; }
        public Resident Resident { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public int Id_HC { get; set; }
        public HealthcareCenter HealthcareCenter { get; set; }

        public int Id_Specialty { get; set; }
        public Specialty Specialty { get; set; }

        public int Id_Companion { get; set; }
        public Employee Companion { get; set; }

        public int Id_StatusAP { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }

        public string Notes { get; set; }
    }


}
