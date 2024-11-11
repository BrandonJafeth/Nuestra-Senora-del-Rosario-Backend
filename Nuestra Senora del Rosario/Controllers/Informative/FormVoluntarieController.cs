using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.DTOS;
using Services.Informative.DTOS.CreatesDto;
using Services.Informative.FormVoluntarieServices;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FormVoluntarieController : ControllerBase
{
    private readonly ISvFormVoluntarieService _formVoluntarieService;
    private readonly IMapper _mapper;

    public FormVoluntarieController(ISvFormVoluntarieService formVoluntarieService, IMapper mapper)
    {
        _formVoluntarieService = formVoluntarieService;
        _mapper = mapper;
    }

    // GET: api/FormVoluntarie
    [HttpGet]
    public async Task<IActionResult> GetFormVoluntaries(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _formVoluntarieService.GetAllFormVoluntariesWithTypeAsync(pageNumber, pageSize);

        return Ok(new
        {
            formVoluntaries = result.FormVoluntaries,
            totalPages = result.TotalPages
        });
    }

    // GET: api/FormVoluntarie/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormVoluntarie(int id)
    {
        var formVoluntarie = await _formVoluntarieService.GetFormVoluntarieWithTypeByIdAsync(id);
        if (formVoluntarie == null)
        {
            return NotFound();
        }

        return Ok(formVoluntarie);
    }

    // POST: api/FormVoluntarie
    [HttpPost]
    public async Task<IActionResult> CreateFormVoluntarie([FromBody] FormVoluntarieCreateDto formVoluntarieCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _formVoluntarieService.CreateFormVoluntarieAsync(formVoluntarieCreateDto);
            return Ok("Formulario de voluntariado creado con éxito.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH: api/FormVoluntarie/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateFormVoluntarieStatus(int id, [FromBody] int statusId)
    {
        try
        {
            await _formVoluntarieService.UpdateFormVoluntarieStatusAsync(id, statusId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/FormVoluntarie/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormVoluntarie(int id)
    {
        await _formVoluntarieService.DeleteFormVoluntarieAsync(id);
        return NoContent();
    }
}
