using DataAccess.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class NursingRequirementsController : ControllerBase
{
    private readonly ISvGenericRepository<NursingRequirements> _nursingRequirementsService;

    public NursingRequirementsController(ISvGenericRepository<NursingRequirements> nursingRequirementsService)
    {
        _nursingRequirementsService = nursingRequirementsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNursingRequirements()
    {
        var items = await _nursingRequirementsService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNursingRequirement(int id)
    {
        var item = await _nursingRequirementsService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddNursingRequirement(NursingRequirements nursingRequirement)
    {
        await _nursingRequirementsService.AddAsync(nursingRequirement);
        await _nursingRequirementsService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNursingRequirement), new { id = nursingRequirement.Id_NursingRequirement }, nursingRequirement);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchNursingRequirement(int id, [FromBody] JsonPatchDocument<NursingRequirements> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _nursingRequirementsService.PatchAsync(id, patchDoc);
        await _nursingRequirementsService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNursingRequirement(int id)
    {
        await _nursingRequirementsService.DeleteAsync(id);
        await _nursingRequirementsService.SaveChangesAsync();
        return NoContent();
    }
}
