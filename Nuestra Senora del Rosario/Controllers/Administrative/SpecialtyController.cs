using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class SpecialtyController : ControllerBase
{
    private readonly ISvGenericRepository<Specialty> _specialtyRepository;
    private readonly IMapper _mapper;

    public SpecialtyController(
        ISvGenericRepository<Specialty> specialtyRepository,
        IMapper mapper)
    {
        _specialtyRepository = specialtyRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var specialties = await _specialtyRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SpecialtyGetDto>>(specialties));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(id);
        if (specialty == null) return NotFound($"Specialty with ID {id} not found.");

        return Ok(_mapper.Map<SpecialtyGetDto>(specialty));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SpecialtyCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var specialty = _mapper.Map<Specialty>(dto);
        await _specialtyRepository.AddAsync(specialty);
        await _specialtyRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = specialty.Id_Specialty },
            _mapper.Map<SpecialtyGetDto>(specialty));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpecialty(int id, [FromBody] SpecialtyUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _specialtyRepository.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Specialty con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _specialtyRepository.SaveChangesAsync();
        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(id);
        if (specialty == null) return NotFound($"Specialty with ID {id} not found.");

        await _specialtyRepository.DeleteAsync(id);
        await _specialtyRepository.SaveChangesAsync();
        return NoContent();
    }
}
