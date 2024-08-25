using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class AboutUsSectionController : ControllerBase
{
    private readonly ISvGenericRepository<AboutUsSection> _aboutUsSectionService;

    public AboutUsSectionController(ISvGenericRepository<AboutUsSection> aboutUsSectionService)
    {
        _aboutUsSectionService = aboutUsSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAboutUsSections()
    {
        var items = await _aboutUsSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAboutUsSection(int id)
    {
        var item = await _aboutUsSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddAboutUsSection(AboutUsSection aboutUsSection)
    {
        await _aboutUsSectionService.AddAsync(aboutUsSection);
        await _aboutUsSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAboutUsSection), new { id = aboutUsSection.Id_About_Us }, aboutUsSection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAboutUsSection(int id, AboutUsSection aboutUsSection)
    {
        if (id != aboutUsSection.Id_About_Us)
        {
            return BadRequest();
        }

        await _aboutUsSectionService.UpdateAsync(aboutUsSection);
        await _aboutUsSectionService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAboutUsSection(int id)
    {
        await _aboutUsSectionService.DeleteAsync(id);
        await _aboutUsSectionService.SaveChangesAsync();
        return NoContent();
    }
}
