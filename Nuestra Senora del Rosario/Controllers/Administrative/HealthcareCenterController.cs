using AutoMapper;
using Entities.Administration;
using Microsoft.AspNetCore.Mvc;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using System.Web.Http.ModelBinding;

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

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] HealthcareCenterCreateUpdateDto dto)
    {
        var existingCenter = await _hcRepository.GetByIdAsync(id);
        if (existingCenter == null) return NotFound($"Center with ID {id} not found.");

        _mapper.Map(dto, existingCenter);
        await _hcRepository.SaveChangesAsync();

        return Ok(_mapper.Map<HealthcareCenterGetDto>(existingCenter));
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
