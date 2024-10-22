using Entities.Administration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.Notifications;
using Services.GenericService;
using Nuestra_Senora_del_Rosario.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Administrative.NotificationServices
{
    public class SvNotification : ISvNotification
    {
        private readonly ISvGenericRepository<Notification> _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext; // Inyectar explícitamente

        public SvNotification(
            ISvGenericRepository<Notification> notificationRepository,
            IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync()
        {
            return await _notificationRepository.Query()
                .Where(n => !n.IsRead)
                .ToListAsync();
        }

        public async Task<Notification> GetNotificationByAppointmentIdAsync(int appointmentId)
        {
            return await _notificationRepository.Query()
                .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);
        }

        // Nuevo: Obtener notificación por título y cita
        public async Task<Notification> GetNotificationByTitleAsync(string title, int appointmentId)
        {
            return await _notificationRepository.Query()
                .FirstOrDefaultAsync(n => n.Title == title && n.AppointmentId == appointmentId);
        }
        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _notificationRepository.SaveChangesAsync();
            }
        }

        public async Task CreateNotificationAsync(string title, string message, int appointmentId)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                AppointmentId = appointmentId
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            // Crear el DTO y enviarlo a los clientes conectados a través del Hub
            var notificationDto = new NotificationGetDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                AppointmentId = notification.AppointmentId,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationDto);
        }
    }
}
