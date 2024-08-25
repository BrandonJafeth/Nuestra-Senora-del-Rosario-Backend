using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class HeroSectionController : ControllerBase
{
    private readonly ISvGenericRepository<HeroSection> _heroSectionService;

    public HeroSectionController(ISvGenericRepository<HeroSection> heroSectionService)
    {
        _heroSectionService = heroSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHeroSections()
    {
        var items = await _heroSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHeroSection(int id)
    {
        var item = await _heroSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddHeroSection(HeroSection heroSection)
    {
        await _heroSectionService.AddAsync(heroSection);
        await _heroSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetHeroSection), new { id = heroSection.Id_Hero }, heroSection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHeroSection(int id, HeroSection heroSection)
    {
        if (id != heroSection.Id_Hero)
        {
            return BadRequest();
        }

        await _heroSectionService.UpdateAsync(heroSection);
        await _heroSectionService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHeroSection(int id)
    {
        await _heroSectionService.DeleteAsync(id);
        await _heroSectionService.SaveChangesAsync();
        return NoContent();
    }
}
