using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class ImportantInformationController : ControllerBase
{
    private readonly ISvGenericRepository<ImportantInformation> _importantInformationService;

    public ImportantInformationController(ISvGenericRepository<ImportantInformation> importantInformationService)
    {
        _importantInformationService = importantInformationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetImportantInformation()
    {
        var items = await _importantInformationService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImportantInfo(int id)
    {
        var item = await _importantInformationService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddImportantInfo(ImportantInformation importantInformation)
    {
        await _importantInformationService.AddAsync(importantInformation);
        await _importantInformationService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetImportantInfo), new { id = importantInformation.Id_ImportantInformation }, importantInformation);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchImportantInfo(int id, [FromBody] JsonPatchDocument<ImportantInformation> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _importantInformationService.PatchAsync(id, patchDoc);
        await _importantInformationService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImportantInfo(int id)
    {
        await _importantInformationService.DeleteAsync(id);
        await _importantInformationService.SaveChangesAsync();
        return NoContent();
    }
}
