using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.Notifications;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ISvNotification _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(ISvNotification notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync();
            var notificationsDto = _mapper.Map<IEnumerable<NotificationGetDto>>(notifications);
            return Ok(notificationsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}
