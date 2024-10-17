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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckAppointments, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void CheckAppointments(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<ISvNotification>();
            var appointmentService = scope.ServiceProvider.GetRequiredService<ISvAppointment>();

            var upcomingAppointments = await appointmentService.GetAllAppointmentsAsync();
            var appointmentsIn3Days = upcomingAppointments
                .Where(a => a.Date == DateTime.UtcNow.AddDays(3).Date);

            foreach (var appointment in appointmentsIn3Days)
            {
                await notificationService.CreateNotificationAsync(
                    "Recordatorio de Cita",
                    $"La cita con {appointment.ResidentFullName} es en 3 días.",
                    appointment.Id_Appointment);
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
