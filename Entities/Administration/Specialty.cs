using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class Specialty
    {
        public int Id_Specialty { get; set; }  // Primary Key
        public string Name_Specialty { get; set; }  // Nombre de la especialidad

        // Relación con citas
        public ICollection<Appointment> Appointments { get; set; }
    }

}
