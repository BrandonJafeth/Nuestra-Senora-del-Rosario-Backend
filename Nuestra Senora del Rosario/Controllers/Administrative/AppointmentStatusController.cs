using AutoMapper;
using Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Web.Http.ModelBinding;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AppointmentStatusCreateUpdateDto dto)
    {
        var existingStatus = await _statusRepository.GetByIdAsync(id);
        if (existingStatus == null) return NotFound($"Status with ID {id} not found.");

        _mapper.Map(dto, existingStatus);
        await _statusRepository.SaveChangesAsync();

        return Ok(_mapper.Map<AppointmentStatusGetDto>(existingStatus));
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
