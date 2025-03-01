using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class VolunteeringSectionController : ControllerBase
{
    private readonly ISvGenericRepository<VolunteeringSection> _volunteeringSectionService;
    private readonly IMapper _mapper;

    public VolunteeringSectionController(ISvGenericRepository<VolunteeringSection> volunteeringSectionService, IMapper mapper)
    {
        _volunteeringSectionService = volunteeringSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetVolunteeringSections()
    {
        var items = await _volunteeringSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVolunteeringSection(int id)
    {
        var item = await _volunteeringSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddVolunteeringSection(VolunteeringSection volunteeringSection)
    {
        await _volunteeringSectionService.AddAsync(volunteeringSection);
        await _volunteeringSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVolunteeringSection), new { id = volunteeringSection.Id_VolunteeringSection }, volunteeringSection);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVolunteeringSection(int id, [FromBody] VolunteeringSectionUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _volunteeringSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"GalleryCategory con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _volunteeringSectionService.SaveChangesAsync();
        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteeringSection(int id)
    {
        await _volunteeringSectionService.DeleteAsync(id);
        await _volunteeringSectionService.SaveChangesAsync();
        return NoContent();
    }
}
