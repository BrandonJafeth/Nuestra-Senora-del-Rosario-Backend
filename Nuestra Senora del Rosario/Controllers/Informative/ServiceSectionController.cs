using Domain.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class ServiceSectionController : ControllerBase
{
    private readonly ISvGenericRepository<ServiceSection> _serviceSectionService;

    public ServiceSectionController(ISvGenericRepository<ServiceSection> serviceSectionService)
    {
        _serviceSectionService = serviceSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetServiceSections()
    {
        var items = await _serviceSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceSection(int id)
    {
        var item = await _serviceSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddServiceSection(ServiceSection serviceSection)
    {
        await _serviceSectionService.AddAsync(serviceSection);
        await _serviceSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetServiceSection), new { id = serviceSection.Id_ServiceSection }, serviceSection);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchServiceSection(int id, [FromBody] JsonPatchDocument<ServiceSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _serviceSectionService.PatchAsync(id, patchDoc);
        await _serviceSectionService.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceSection(int id)
    {
        await _serviceSectionService.DeleteAsync(id);
        await _serviceSectionService.SaveChangesAsync();
        return NoContent();
    }
}
