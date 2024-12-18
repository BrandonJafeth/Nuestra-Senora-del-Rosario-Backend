using DataAccess.Entities.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.Notifications
{
    public interface ISvNotification
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(); // Notificaciones no leídas
        Task MarkAsReadAsync(int id); // Marcar notificación como leída
        Task CreateNotificationAsync(string title, string message, int appointmentId); // Crear notificación
        Task<Notification> GetNotificationByAppointmentIdAsync(int appointmentId);
        Task<Notification> GetNotificationByTitleAsync(string title, int appointmentId);


    }
}
