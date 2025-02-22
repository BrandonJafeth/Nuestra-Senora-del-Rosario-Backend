using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class AppointmentMinimalDto
    {
        public int Id_Appointment { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
