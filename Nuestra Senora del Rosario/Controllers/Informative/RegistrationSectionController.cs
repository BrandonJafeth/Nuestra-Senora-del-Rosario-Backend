using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class RegistrationSectionController : ControllerBase
{
    private readonly ISvGenericRepository<RegistrationSection> _registrationSectionService;

    public RegistrationSectionController(ISvGenericRepository<RegistrationSection> registrationSectionService)
    {
        _registrationSectionService = registrationSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegistrationSections()
    {
        var items = await _registrationSectionService.GetAllAsync();
        return Ok(items);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchRegistrationSection(int id, [FromBody] JsonPatchDocument<RegistrationSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _registrationSectionService.PatchAsync(id, patchDoc);
        await _registrationSectionService.SaveChangesAsync();

        return NoContent();
    }


}
