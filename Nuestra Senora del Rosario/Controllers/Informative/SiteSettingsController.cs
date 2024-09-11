using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class SiteSettingsController : ControllerBase
{
    private readonly ISvGenericRepository<SiteSettings> _siteSettingsService;

    public SiteSettingsController(ISvGenericRepository<SiteSettings> siteSettingsService)
    {
        _siteSettingsService = siteSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSiteSettings()
    {
        var items = await _siteSettingsService.GetAllAsync();
        return Ok(items);
    }




    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchSiteSetting(int id, [FromBody] JsonPatchDocument<SiteSettings> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _siteSettingsService.PatchAsync(id, patchDoc);
        await _siteSettingsService.SaveChangesAsync();

        return NoContent();
    }


}
