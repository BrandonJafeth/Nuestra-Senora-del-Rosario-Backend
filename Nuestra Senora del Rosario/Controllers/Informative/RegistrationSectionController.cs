using Entities.Informative;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegistrationSection(int id)
    {
        var item = await _registrationSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddRegistrationSection(RegistrationSection registrationSection)
    {
        await _registrationSectionService.AddAsync(registrationSection);
        await _registrationSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRegistrationSection), new { id = registrationSection.Id_RegistrationSection }, registrationSection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRegistrationSection(int id, RegistrationSection registrationSection)
    {
        if (id != registrationSection.Id_RegistrationSection)
        {
            return BadRequest();
        }

        await _registrationSectionService.UpdateAsync(registrationSection);
        await _registrationSectionService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegistrationSection(int id)
    {
        await _registrationSectionService.DeleteAsync(id);
        await _registrationSectionService.SaveChangesAsync();
        return NoContent();
    }
}
