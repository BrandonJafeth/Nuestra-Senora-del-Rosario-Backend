using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.Appointments;

namespace Infrastructure.Services.Administrative.Notifications
{
    public class NotificationHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public NotificationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Iniciar el servicio con un temporizador que se ejecuta cada hora
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckAppointments, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void CheckAppointments(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<ISvNotification>();
            var appointmentService = scope.ServiceProvider.GetRequiredService<ISvAppointment>();

            var now = DateTime.UtcNow;
            var upcomingAppointments = await appointmentService.GetAllAppointmentsAsync();

            // Filtrar citas dentro de los próximos 3 días
            var relevantAppointments = upcomingAppointments
                .Where(a => a.Date >= now.Date && a.Date <= now.AddDays(3).Date)
                .OrderBy(a => a.Date).ThenBy(a => a.Time)
                .ToList();

            foreach (var appointment in relevantAppointments)
            {
                var daysLeft = (appointment.Date - now.Date).Days; // Calcular días restantes
                var note = string.IsNullOrWhiteSpace(appointment.Notes)
                    ? "No hay notas adicionales."
                    : appointment.Notes;

                // Verificar si ya se notificó para este día específico
                var notificationTitle = $"Recordatorio de Cita ({daysLeft} días restantes)";
                var existingNotification = await notificationService.GetNotificationByTitleAsync(notificationTitle, appointment.Id_Appointment);

                if (existingNotification == null) // Crear notificación si no existe para el día específico
                {
                    var message = daysLeft switch
                    {
                        0 => $"La cita con {appointment.ResidentFullName} es hoy. Nota: {note}",
                        1 => $"La cita con {appointment.ResidentFullName} es mañana. Nota: {note}",
                        _ => $"La cita con {appointment.ResidentFullName} es en {daysLeft} días. Nota: {note}"
                    };

                    await notificationService.CreateNotificationAsync(
                        notificationTitle,  // Título único por día restante
                        message,
                        appointment.Id_Appointment
                    );
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
