using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

namespace Infrastructure.Services.Administrative.Notifications.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(NotificationGetDto notification)
        {
            // Enviar notificación a todos los clientes conectados
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
