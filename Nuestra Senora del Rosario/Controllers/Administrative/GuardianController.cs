using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Entities.Administration;
using Microsoft.EntityFrameworkCore;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuardianController : ControllerBase
    {
        private readonly ISvGenericRepository<Guardian> _guardianService;

        public GuardianController(ISvGenericRepository<Guardian> guardianService)
        {
            _guardianService = guardianService;
        }

        // GET: api/guardian
        [HttpGet]
        public async Task<IActionResult> GetAllGuardians()
        {
            var guardians = await _guardianService.GetAllAsync();
            return Ok(guardians);
        }

        // GET: api/guardian/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuardianById(int id)
        {
            var guardian = await _guardianService.GetByIdAsync(id);
            if (guardian == null)
            {
                return NotFound($"Guardian with ID {id} not found.");
            }
            return Ok(guardian);
        }

        // POST: api/guardian
        [HttpPost]
        public async Task<IActionResult> CreateGuardian([FromBody] Guardian guardian)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _guardianService.AddAsync(guardian);
            await _guardianService.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuardianById), new { id = guardian.Id_Guardian }, guardian);
        }

        // PATCH: api/guardian/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchGuardian(int id, [FromBody] JsonPatchDocument<Guardian> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            try
            {
                await _guardianService.PatchAsync(id, patchDoc);
                await _guardianService.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        // DELETE: api/guardian/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuardian(int id)
        {
            await _guardianService.DeleteAsync(id);
            await _guardianService.SaveChangesAsync();
            return NoContent();
        }
    }
}
