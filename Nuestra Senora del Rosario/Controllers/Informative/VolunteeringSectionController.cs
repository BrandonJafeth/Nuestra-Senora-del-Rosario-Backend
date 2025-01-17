using Domain.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class VolunteeringSectionController : ControllerBase
{
    private readonly ISvGenericRepository<VolunteeringSection> _volunteeringSectionService;

    public VolunteeringSectionController(ISvGenericRepository<VolunteeringSection> volunteeringSectionService)
    {
        _volunteeringSectionService = volunteeringSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVolunteeringSections()
    {
        var items = await _volunteeringSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVolunteeringSection(int id)
    {
        var item = await _volunteeringSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddVolunteeringSection(VolunteeringSection volunteeringSection)
    {
        await _volunteeringSectionService.AddAsync(volunteeringSection);
        await _volunteeringSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVolunteeringSection), new { id = volunteeringSection.Id_VolunteeringSection }, volunteeringSection);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchVolunteeringSection(int id, [FromBody] JsonPatchDocument<VolunteeringSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _volunteeringSectionService.PatchAsync(id, patchDoc);
        await _volunteeringSectionService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteeringSection(int id)
    {
        await _volunteeringSectionService.DeleteAsync(id);
        await _volunteeringSectionService.SaveChangesAsync();
        return NoContent();
    }
}
