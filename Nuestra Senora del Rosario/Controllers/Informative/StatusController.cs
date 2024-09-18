using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nuestra_Senora_del_Rosario.Controllers.Informative
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ISvGenericRepository<Status> _statusService;

        public StatusController(ISvGenericRepository<Status> statusService)
        {
            _statusService = statusService;
        }

        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
        {
            var statuses = await _statusService.GetAllAsync();
            return Ok(statuses);
        }

        // GET: api/Status/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatus(int id)
        {
            var status = await _statusService.GetByIdAsync(id);

            if (status == null)
            {
                return NotFound($"Status with ID {id} not found.");
            }

            return Ok(status);
        }
    }
}
