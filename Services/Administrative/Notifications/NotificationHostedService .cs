using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Services.Administrative.AppointmentService;
using Services.Administrative.Notifications;

namespace Services.Administrative.NotificationServices
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

        // Método que verifica las citas y envía notificaciones
        private async void CheckAppointments(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<ISvNotification>();
            var appointmentService = scope.ServiceProvider.GetRequiredService<ISvAppointment>();

            var upcomingAppointments = await appointmentService.GetAllAppointmentsAsync();

            // Obtener las citas en los próximos 3 días
            var now = DateTime.UtcNow;
            var relevantAppointments = upcomingAppointments
                .Where(a => a.Date >= now.Date && a.Date <= now.AddDays(3).Date)
                .OrderBy(a => a.Date).ThenBy(a => a.Time) // Ordenar por fecha y hora
                .Take(2) // Limitar a 2 notificaciones por ejecución
                .ToList();

            // Enviar un máximo de 2 notificaciones por día
            foreach (var appointment in relevantAppointments)
            {
                var daysLeft = (appointment.Date - now.Date).Days; // Días restantes
                var message = daysLeft switch
                {
                    0 => $"La cita con {appointment.ResidentFullName} es hoy.",
                    1 => $"La cita con {appointment.ResidentFullName} es mañana.",
                    _ => $"La cita con {appointment.ResidentFullName} es en {daysLeft} días."
                };

                await notificationService.CreateNotificationAsync(
                    "Recordatorio de Cita",
                    message,
                    appointment.Id_Appointment
                );
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
