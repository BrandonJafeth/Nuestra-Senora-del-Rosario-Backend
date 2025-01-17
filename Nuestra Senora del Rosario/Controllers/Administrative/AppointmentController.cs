using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.Appointments;
using Microsoft.AspNetCore.Mvc;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Validations.Admistrative;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ISvAppointment _appointmentService;
    private readonly IValidator<AppointmentPostDto> _appointmentPostDtoValidator;

    // Inyección de dependencias del servicio y validador
    public AppointmentController(
        ISvAppointment appointmentService,
        IValidator<AppointmentPostDto> appointmentPostDtoValidator)
    {
        _appointmentService = appointmentService;
        _appointmentPostDtoValidator = appointmentPostDtoValidator;
    }

    // GET: api/appointment
    [HttpGet]
    public async Task<IActionResult> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    // GET: api/appointment/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound($"Cita con ID {id} no encontrada.");
        return Ok(appointment);
    }

    // POST: api/appointment
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentPostDto appointmentDto)
    {
        ValidationResult result = await _appointmentPostDtoValidator.ValidateAsync(appointmentDto);

        if (!result.IsValid)
        {
            return BadRequest(result.Errors); // Retorna errores de validación
        }

        await _appointmentService.CreateAppointmentAsync(appointmentDto);
        return Ok("Cita creada exitosamente.");
    }

    // PUT: api/appointment/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentUpdateDto appointmentDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            appointmentDto.Id_Appointment = id;
            await _appointmentService.UpdateAppointmentAsync(appointmentDto);
            return Ok(new { message = "Cita actualizada correctamente." });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAppointment(int id, [FromBody] AppointmentUpdateDto appointmentDto)
    {
        try
        {
            await _appointmentService.PatchAppointmentAsync(id, appointmentDto);
            return Ok(new { message = "Cita actualizada correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // DELETE: api/appointment/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        try
        {
            await _appointmentService.DeleteAppointmentAsync(id);
            return NoContent();
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
