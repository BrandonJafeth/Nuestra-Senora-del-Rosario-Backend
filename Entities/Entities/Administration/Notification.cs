using System;
using System.Runtime.InteropServices; // Para detectar el sistema operativo

namespace Domain.Entities.Administration
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = GetCostaRicaTime();

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        private static DateTime GetCostaRicaTime()
        {
            // Detectar si el sistema operativo es Windows o Linux
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Central America Standard Time"   // ID para Windows
                : "America/Costa_Rica";             // ID para Linux

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}
