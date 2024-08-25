using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSiteSetting(int id)
    {
        var item = await _siteSettingsService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddSiteSetting(SiteSettings siteSettings)
    {
        await _siteSettingsService.AddAsync(siteSettings);
        await _siteSettingsService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSiteSetting), new { id = siteSettings.Id_Site_Settings }, siteSettings);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSiteSetting(int id, SiteSettings siteSettings)
    {
        if (id != siteSettings.Id_Site_Settings)
        {
            return BadRequest();
        }

        await _siteSettingsService.UpdateAsync(siteSettings);
        await _siteSettingsService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSiteSetting(int id)
    {
        await _siteSettingsService.DeleteAsync(id);
        await _siteSettingsService.SaveChangesAsync();
        return NoContent();
    }
}
