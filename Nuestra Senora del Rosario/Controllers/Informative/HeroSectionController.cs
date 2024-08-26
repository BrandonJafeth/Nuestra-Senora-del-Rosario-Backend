using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchHeroSection(int id, [FromBody] JsonPatchDocument<HeroSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _heroSectionService.PatchAsync(id, patchDoc);
        await _heroSectionService.SaveChangesAsync();

        return NoContent();
    }


}
