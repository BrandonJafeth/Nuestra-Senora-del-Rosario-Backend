using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class HealthcareCenterController : ControllerBase
{
    private readonly ISvGenericRepository<HealthcareCenter> _hcRepository;
    private readonly IMapper _mapper;

    public HealthcareCenterController(
        ISvGenericRepository<HealthcareCenter> hcRepository,
        IMapper mapper)
    {
        _hcRepository = hcRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var centers = await _hcRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<HealthcareCenterGetDto>>(centers));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var center = await _hcRepository.GetByIdAsync(id);
        if (center == null) return NotFound($"Center with ID {id} not found.");

        return Ok(_mapper.Map<HealthcareCenterGetDto>(center));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HealthcareCenterCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var center = _mapper.Map<HealthcareCenter>(dto);
        await _hcRepository.AddAsync(center);
        await _hcRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = center.Id_HC },
            _mapper.Map<HealthcareCenterGetDto>(center));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHealthCareCenter(int id, [FromBody] HealthcareCenterUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _hcRepository.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"HealthcareCenter con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _hcRepository.SaveChangesAsync();
        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var center = await _hcRepository.GetByIdAsync(id);
        if (center == null) return NotFound($"Center with ID {id} not found.");

        await _hcRepository.DeleteAsync(id);
        await _hcRepository.SaveChangesAsync();
        return NoContent();
    }
}
