using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Administrative.Guardians;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuardianController : ControllerBase
    {
        private readonly ISvGuardian _guardianService;

        public GuardianController(ISvGuardian guardianService)
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
                return NotFound($"Guardian con ID {id} no encontrado.");
            }
            return Ok(guardian);
        }

        // **Nuevo**: Buscar guardianes por nombre o apellido
        [HttpGet("search")]
        public async Task<IActionResult> SearchGuardiansByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Debe proporcionar un nombre o apellido para buscar.");
            }

            var guardians = await _guardianService.SearchGuardiansByNameAsync(name);

            if (guardians == null || !guardians.Any())
            {
                return NotFound($"No se encontraron guardianes con el nombre o apellido '{name}'.");
            }

            return Ok(guardians);
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
                return BadRequest("Documento de actualización inválido.");
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
            var guardian = await _guardianService.GetByIdAsync(id);
            if (guardian == null)
            {
                return NotFound($"Guardian con ID {id} no encontrado.");
            }

            await _guardianService.DeleteAsync(id);
            await _guardianService.SaveChangesAsync();
            return NoContent();
        }
    }
}
