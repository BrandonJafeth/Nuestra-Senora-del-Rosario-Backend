using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Administration
{
    public class HealthcareCenter
    {
        public int Id_HC { get; set; }  // Primary Key
        public string Name_HC { get; set; }  // Nombre del centro
        public string Location_HC { get; set; }  // Dirección del centro
        public string Type_HC { get; set; }  // Tipo del centro (Public, Private)

        // Relación con citas
        public ICollection<Appointment> Appointments { get; set; }
    }

}
