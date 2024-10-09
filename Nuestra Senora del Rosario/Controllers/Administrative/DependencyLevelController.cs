using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Entities.Administration;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DependencyLevelController : ControllerBase
{
    private readonly ISvGenericRepository<DependencyLevel> _dependencyLevelService;

    public DependencyLevelController(ISvGenericRepository<DependencyLevel> dependencyLevelService)
    {
        _dependencyLevelService = dependencyLevelService;
    }

    // GET: api/dependencylevel
    [HttpGet]
    public async Task<IActionResult> GetAllDependencyLevels()
    {
        var dependencyLevels = await _dependencyLevelService.GetAllAsync();
        return Ok(dependencyLevels);
    }

    // GET: api/dependencylevel/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDependencyLevelById(int id)
    {
        var dependencyLevel = await _dependencyLevelService.GetByIdAsync(id);
        if (dependencyLevel == null)
        {
            return NotFound($"Dependency Level with ID {id} not found.");
        }
        return Ok(dependencyLevel);
    }

    // POST: api/dependencylevel
    [HttpPost]
    public async Task<IActionResult> CreateDependencyLevel([FromBody] DependencyLevel dependencyLevel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _dependencyLevelService.AddAsync(dependencyLevel);
        await _dependencyLevelService.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDependencyLevelById), new { id = dependencyLevel.Id_DependencyLevel }, dependencyLevel);
    }

    // PATCH: api/dependencylevel/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDependencyLevel(int id, [FromBody] JsonPatchDocument<DependencyLevel> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        try
        {
            await _dependencyLevelService.PatchAsync(id, patchDoc);
            await _dependencyLevelService.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    // DELETE: api/dependencylevel/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDependencyLevel(int id)
    {
        await _dependencyLevelService.DeleteAsync(id);
        await _dependencyLevelService.SaveChangesAsync();
        return NoContent();
    }
}
