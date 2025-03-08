using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class VoluntarieTypeController : ControllerBase
{
    private readonly ISvGenericRepository<VoluntarieType> _voluntarieTypeRepository;
    private readonly IMapper _mapper;

    public VoluntarieTypeController(ISvGenericRepository<VoluntarieType> voluntarieTypeRepository, IMapper mapper)
    {
        _voluntarieTypeRepository = voluntarieTypeRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetVoluntarieTypes()
    {
        var voluntarieTypes = await _voluntarieTypeRepository.GetAllAsync();
        var voluntarieTypeDtos = _mapper.Map<IEnumerable<VoluntarieTypeDto>>(voluntarieTypes);
        return Ok(voluntarieTypeDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVoluntarieType(int id)
    {
        var voluntarieType = await _voluntarieTypeRepository.GetByIdAsync(id);
        if (voluntarieType == null)
        {
            return NotFound();
        }

        var voluntarieTypeDto = _mapper.Map<VoluntarieTypeDto>(voluntarieType);
        return Ok(voluntarieTypeDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddVoluntarieType(VoluntarieTypeDto voluntarieTypeDto)
    {
        var voluntarieType = _mapper.Map<VoluntarieType>(voluntarieTypeDto);
        await _voluntarieTypeRepository.AddAsync(voluntarieType);
        await _voluntarieTypeRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVoluntarieType), new { id = voluntarieType.Id_VoluntarieType }, voluntarieType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVoluntarieType(int id, [FromBody] VoluntarieTypeUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _voluntarieTypeRepository.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"VoluntarieType con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _voluntarieTypeRepository.SaveChangesAsync();
        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVoluntarieType(int id)
    {
        await _voluntarieTypeRepository.DeleteAsync(id);
        await _voluntarieTypeRepository.SaveChangesAsync();
        return NoContent();
    }
}
