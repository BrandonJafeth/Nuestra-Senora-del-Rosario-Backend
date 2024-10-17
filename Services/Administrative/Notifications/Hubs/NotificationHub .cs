using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

namespace Nuestra_Senora_del_Rosario.Hubs
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
