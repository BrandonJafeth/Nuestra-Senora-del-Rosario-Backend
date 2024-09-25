using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nuestra_Senora_del_Rosario.Controllers.Informative
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationStatusController : ControllerBase
    {
        private readonly ISvGenericRepository<ApplicationStatus> _statusAppllicationService;

        public ApplicationStatusController(ISvGenericRepository<ApplicationStatus> statusAppllicationService)
        {
            _statusAppllicationService = statusAppllicationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationStatus>>> GetApplicationStatus()
        {
            var applicationstatuses = await _statusAppllicationService.GetAllAsync();
            return Ok(applicationstatuses);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationStatus>> GetApplicationStatus(int id)
        {
            var applicationstatuses = await _statusAppllicationService.GetByIdAsync(id);

            if (applicationstatuses == null)
            {
                return NotFound($"Status with ID {id} not found.");
            }

            return Ok(applicationstatuses);
        }
    }
}