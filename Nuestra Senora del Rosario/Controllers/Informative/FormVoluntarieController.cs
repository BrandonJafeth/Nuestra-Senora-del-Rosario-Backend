using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.DTOS.CreatesDto;
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

    [HttpGet]
    public async Task<IActionResult> GetFormVoluntaries()
    {
        var formVoluntaries = await _formVoluntarieService.GetFormVoluntariesWithTypesAsync();
        var formVoluntarieDtos = _mapper.Map<IEnumerable<FormVoluntarieDto>>(formVoluntaries);
        return Ok(formVoluntarieDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormVoluntarie(int id)
    {
        var formVoluntarie = await _formVoluntarieService.GetByIdAsync(id);
        if (formVoluntarie == null)
        {
            return NotFound();
        }

        var formVoluntarieDto = _mapper.Map<FormVoluntarieDto>(formVoluntarie);
        return Ok(formVoluntarieDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddFormVoluntarie(FormVoluntarieCreateDto formVoluntarieCreateDto)
    {
        var formVoluntarie = _mapper.Map<FormVoluntarie>(formVoluntarieCreateDto);
        await _formVoluntarieService.AddAsync(formVoluntarie);
        await _formVoluntarieService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFormVoluntarie), new { id = formVoluntarie.Id_FormVoluntarie }, formVoluntarie);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchFormVoluntarie(int id, [FromBody] JsonPatchDocument<FormVoluntarie> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _formVoluntarieService.PatchAsync(id, patchDoc);
        await _formVoluntarieService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormVoluntarie(int id)
    {
        await _formVoluntarieService.DeleteAsync(id);
        await _formVoluntarieService.SaveChangesAsync();
        return NoContent();
    }
}
