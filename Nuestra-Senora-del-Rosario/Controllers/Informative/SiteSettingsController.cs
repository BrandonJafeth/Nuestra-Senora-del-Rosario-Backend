using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class SiteSettingsController : ControllerBase
{
    private readonly ISvGenericRepository<SiteSettings> _siteSettingsService;
    private readonly IMapper _mapper;

    public SiteSettingsController(ISvGenericRepository<SiteSettings> siteSettingsService, IMapper mapper)
    {
        _siteSettingsService = siteSettingsService;
        _mapper = mapper;

    }

    [HttpGet]
    public async Task<IActionResult> GetSiteSettings()
    {
        var items = await _siteSettingsService.GetAllAsync();
        return Ok(items);
    }




    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSiteSettings(int id, [FromBody] SiteSettingsUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _siteSettingsService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"SiteSettings con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _siteSettingsService.SaveChangesAsync();
        return Ok(existingSection);
    }


}
