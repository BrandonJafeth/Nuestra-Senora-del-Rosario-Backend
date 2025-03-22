using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Enfermeria")]  // <-- Aquí especificas el rol
public class AppointmentStatusController : ControllerBase
{
    private readonly ISvGenericRepository<AppointmentStatus> _statusRepository;
    private readonly IMapper _mapper;

    public AppointmentStatusController(
        ISvGenericRepository<AppointmentStatus> statusRepository,
        IMapper mapper)
    {
        _statusRepository = statusRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _statusRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<AppointmentStatusGetDto>>(statuses));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _statusRepository.GetByIdAsync(id);
        if (status == null) return NotFound($"Status with ID {id} not found.");

        return Ok(_mapper.Map<AppointmentStatusGetDto>(status));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppointmentStatusCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var status = _mapper.Map<AppointmentStatus>(dto);
        await _statusRepository.AddAsync(status);
        await _statusRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = status.Id_StatusAP },
            _mapper.Map<AppointmentStatusGetDto>(status));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatusUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _statusRepository.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"AppointmentStatus con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _statusRepository.SaveChangesAsync();

        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var status = await _statusRepository.GetByIdAsync(id);
        if (status == null) return NotFound($"Status with ID {id} not found.");

        await _statusRepository.DeleteAsync(id);
        await _statusRepository.SaveChangesAsync();
        return NoContent();
    }
}
