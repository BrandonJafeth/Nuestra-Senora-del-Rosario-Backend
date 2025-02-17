using Domain.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
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


}
