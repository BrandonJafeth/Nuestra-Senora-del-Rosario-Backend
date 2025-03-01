using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class HeroSectionController : ControllerBase
{
    private readonly ISvGenericRepository<HeroSection> _heroSectionService;
    private readonly IMapper _mapper;

    public HeroSectionController(ISvGenericRepository<HeroSection> heroSectionService, IMapper mapper)
    {
        _heroSectionService = heroSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetHeroSections()
    {
        var items = await _heroSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonationsSection(int id, [FromBody] HeroSectionUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _heroSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"HeroSection con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _heroSectionService.SaveChangesAsync();
        return Ok(existingSection);
    }


}
