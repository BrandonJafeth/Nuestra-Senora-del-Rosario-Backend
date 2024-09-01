using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AssociatesSectionController : ControllerBase
{
    private readonly ISvGenericRepository<AssociatesSection> _associatesSectionService;

    public AssociatesSectionController(ISvGenericRepository<AssociatesSection> associatesSectionService)
    {
        _associatesSectionService = associatesSectionService;
    }

    // GET: api/AssociatesSection
    [HttpGet]
    public async Task<IActionResult> GetAssociatesSections()
    {
        var items = await _associatesSectionService.GetAllAsync();
        return Ok(items);
    }

    // GET: api/AssociatesSection/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssociatesSection(int id)
    {
        var item = await _associatesSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // POST: api/AssociatesSection
    [HttpPost]
    public async Task<IActionResult> AddAssociatesSection(AssociatesSection associatesSection)
    {
        await _associatesSectionService.AddAsync(associatesSection);
        await _associatesSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAssociatesSection), new { id = associatesSection.Id_AssociatesSection }, associatesSection);
    }

    // PATCH: api/AssociatesSection/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAssociatesSection(int id, [FromBody] JsonPatchDocument<AssociatesSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _associatesSectionService.PatchAsync(id, patchDoc);
        await _associatesSectionService.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/AssociatesSection/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssociatesSection(int id)
    {
        await _associatesSectionService.DeleteAsync(id);
        await _associatesSectionService.SaveChangesAsync();
        return NoContent();
    }
}
