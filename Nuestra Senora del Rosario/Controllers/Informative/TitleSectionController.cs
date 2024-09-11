using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class TitleSectionController : ControllerBase
{
    private readonly ISvGenericRepository<TitleSection> _titleSectionService;

    public TitleSectionController(ISvGenericRepository<TitleSection> titleSectionService)
    {
        _titleSectionService = titleSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTitleSections()
    {
        var items = await _titleSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTitleSection(int id)
    {
        var item = await _titleSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddTitleSection(TitleSection titleSection)
    {
        await _titleSectionService.AddAsync(titleSection);
        await _titleSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTitleSection), new { id = titleSection.Id_TitleSection }, titleSection);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchTitleSection(int id, [FromBody] JsonPatchDocument<TitleSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _titleSectionService.PatchAsync(id, patchDoc);
        await _titleSectionService.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTitleSection(int id)
    {
        await _titleSectionService.DeleteAsync(id);
        await _titleSectionService.SaveChangesAsync();
        return NoContent();
    }
}
