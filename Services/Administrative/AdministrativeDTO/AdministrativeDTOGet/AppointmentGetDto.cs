using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class AppointmentGetDto
    {
        public int Id_Appointment { get; set; }  // Identificador de la cita

        // Información del residente
        public string ResidentFullName { get; set; }  // Nombre completo del residente
        public string ResidentCedula { get; set; }  // Cédula del residente

        public TimeSpan Time { get; set; }  // Hora de la cita
        public DateTime Date { get; set; }  // Fecha de la cita

        // Especialidad y Centro de Atención
        public string SpecialtyName { get; set; }  // Nombre de la especialidad
        public string HealthcareCenterName { get; set; }  // Nombre del centro de atención

        // Información del acompañante
        public string CompanionName { get; set; }  // Nombre del acompañante (Empleado)

        public string StatusName { get; set; }  // Estado de la cita (Pendiente, Completada, etc.)

        public string Notes { get; set; }  // Notas adicionales (opcional)
    }

}
