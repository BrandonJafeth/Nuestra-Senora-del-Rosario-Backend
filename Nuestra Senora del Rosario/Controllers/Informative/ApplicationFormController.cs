using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.DTOS.CreatesDto;
using Services.Informative.ApplicationFormService;
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
    public async Task<IActionResult> GetApplicationForms()
    {
        var applicationForms = await _applicationFormService.GetAllFormsAsync();
        var applicationFormDtos = _mapper.Map<IEnumerable<ApplicationFormDto>>(applicationForms);
        return Ok(applicationFormDtos);
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
}
