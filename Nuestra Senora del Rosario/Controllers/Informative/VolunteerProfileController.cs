using DataAccess.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class VolunteerProfileController : ControllerBase
{
    private readonly ISvGenericRepository<VolunteerProfile> _volunteerProfileService;

    public VolunteerProfileController(ISvGenericRepository<VolunteerProfile> volunteerProfileService)
    {
        _volunteerProfileService = volunteerProfileService;
    }

    // GET: api/VolunteerProfile
    [HttpGet]
    public async Task<IActionResult> GetVolunteerProfiles()
    {
        var items = await _volunteerProfileService.GetAllAsync();
        return Ok(items);
    }

    // GET: api/VolunteerProfile/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVolunteerProfile(int id)
    {
        var item = await _volunteerProfileService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // POST: api/VolunteerProfile
    [HttpPost]
    public async Task<IActionResult> AddVolunteerProfile(VolunteerProfile volunteerProfile)
    {
        await _volunteerProfileService.AddAsync(volunteerProfile);
        await _volunteerProfileService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVolunteerProfile), new { id = volunteerProfile.Id_Volunteer_Profile }, volunteerProfile);
    }

    // PATCH: api/VolunteerProfile/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchVolunteerProfile(int id, [FromBody] JsonPatchDocument<VolunteerProfile> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _volunteerProfileService.PatchAsync(id, patchDoc);
        await _volunteerProfileService.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/VolunteerProfile/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteerProfile(int id)
    {
        await _volunteerProfileService.DeleteAsync(id);
        await _volunteerProfileService.SaveChangesAsync();
        return NoContent();
    }
}
