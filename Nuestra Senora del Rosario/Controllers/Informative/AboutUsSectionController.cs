using DataAccess.Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

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



    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAboutUsSection(int id, [FromBody] JsonPatchDocument<AboutUsSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _aboutUsSectionService.PatchAsync(id, patchDoc);
        await _aboutUsSectionService.SaveChangesAsync();

        return NoContent();
    }

}
