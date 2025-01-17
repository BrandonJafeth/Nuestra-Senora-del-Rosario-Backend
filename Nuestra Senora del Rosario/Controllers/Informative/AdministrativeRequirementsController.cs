using Domain.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class AdministrativeRequirementsController : ControllerBase
{
    private readonly ISvGenericRepository<AdministrativeRequirements> _administrativeRequirementsService;

    public AdministrativeRequirementsController(ISvGenericRepository<AdministrativeRequirements> administrativeRequirementsService)
    {
        _administrativeRequirementsService = administrativeRequirementsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAdministrativeRequirements()
    {
        var items = await _administrativeRequirementsService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdministrativeRequirement(int id)
    {
        var item = await _administrativeRequirementsService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddAdministrativeRequirement(AdministrativeRequirements requirement)
    {
        await _administrativeRequirementsService.AddAsync(requirement);
        await _administrativeRequirementsService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAdministrativeRequirement), new { id = requirement.Id_AdministrativeRequirement }, requirement);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAdministrativeRequirement(int id, [FromBody] JsonPatchDocument<AdministrativeRequirements> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _administrativeRequirementsService.PatchAsync(id, patchDoc);
        await _administrativeRequirementsService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrativeRequirement(int id)
    {
        await _administrativeRequirementsService.DeleteAsync(id);
        await _administrativeRequirementsService.SaveChangesAsync();
        return NoContent();
    }
}
