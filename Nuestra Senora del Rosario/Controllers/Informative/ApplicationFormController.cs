using AutoMapper;
using Infrastructure.Services.Informative.ApplicationFormService;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ApplicationFormController : ControllerBase
{
    private readonly ISvApplicationForm _applicationFormService;
    private readonly IMapper _mapper;

    public ApplicationFormController(ISvApplicationForm applicationFormService, IMapper mapper)
    {
        _applicationFormService = applicationFormService;
        _mapper = mapper;
    }

    // GET: api/ApplicationForm
    [HttpGet]
    public async Task<IActionResult> GetAllForms(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _applicationFormService.GetAllFormsAsync(pageNumber, pageSize);

        return Ok(new
        {
            forms = result.Forms,
            totalPages = result.TotalPages
        });
    }


    // GET: api/ApplicationForm/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationForm(int id)
    {
        var applicationForm = await _applicationFormService.GetFormByIdAsync(id);
        if (applicationForm == null)
        {
            return NotFound();
        }

        var applicationFormDto = _mapper.Map<ApplicationFormDto>(applicationForm);
        return Ok(applicationFormDto);
    }

    // POST: api/ApplicationForm
    [HttpPost]
    [EnableRateLimiting("LimiteDeSolicitudes")]
    public async Task<IActionResult> AddApplicationForm([FromBody] ApplicationFormCreateDto applicationFormCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Usa el servicio para manejar la lógica de creación, no el mapeo directo
        await _applicationFormService.AddFormAsync(applicationFormCreateDto);
        return CreatedAtAction(nameof(GetApplicationForm), new { id = applicationFormCreateDto.Id_ApplicationForm }, applicationFormCreateDto);
    }

    // DELETE: api/ApplicationForm/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplicationForm(int id)
    {
        var applicationForm = await _applicationFormService.GetFormByIdAsync(id);
        if (applicationForm == null)
        {
            return NotFound();
        }

        await _applicationFormService.DeleteAsync(id);
        return NoContent();
    }

    // PATCH: api/ApplicationForm/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateApplicationFormStatus(int id, [FromBody] int statusId)
    {
        if (statusId <= 0)
        {
            return BadRequest("El estado proporcionado no es válido.");
        }

        try
        {
            await _applicationFormService.UpdateFormStatusAsync(id, statusId);
            return NoContent(); // Retorna 204 No Content si se actualizó correctamente
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); // Retorna 404 Not Found si el formulario no existe
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // Retorna 400 Bad Request si el estado no es válido
        }
    }
}
