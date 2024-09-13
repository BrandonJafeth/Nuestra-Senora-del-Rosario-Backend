using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.GenericService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProfessionController : ControllerBase
{
    private readonly ISvGenericRepository<Profession> _professionService;

    public ProfessionController(ISvGenericRepository<Profession> professionService)
    {
        _professionService = professionService;
    }

    // POST: api/profession
    [HttpPost]
    public async Task<IActionResult> CreateProfession([FromBody] Profession profession)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Agregar la profesión directamente desde el objeto recibido
            await _professionService.AddAsync(profession);
            await _professionService.SaveChangesAsync();

            return Ok(profession); // Devuelve el objeto creado
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }


    // GET: api/profession/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfessionById(int id)
    {
        var profession = await _professionService.GetByIdAsync(id);
        if (profession == null)
        {
            return NotFound($"Profession with ID {id} not found.");
        }
        return Ok(profession);
    }

    // GET: api/profession
    [HttpGet]
    public async Task<IActionResult> GetAllProfessions()
    {
        var professions = await _professionService.GetAllAsync();
        return Ok(professions);
    }

    // PATCH: api/profession/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchProfession(int id, [FromBody] JsonPatchDocument<Profession> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        var profession = await _professionService.GetByIdAsync(id);
        if (profession == null)
        {
            return NotFound($"Profession with ID {id} not found.");
        }

        patchDoc.ApplyTo(profession, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _professionService.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/profession/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfession(int id)
    {
        // Llamamos al método DeleteAsync directamente desde el servicio
        await _professionService.DeleteAsync(id);

        // Guardamos los cambios
        await _professionService.SaveChangesAsync();

        return NoContent(); // Respuesta 204 si todo fue exitoso
    }


}
