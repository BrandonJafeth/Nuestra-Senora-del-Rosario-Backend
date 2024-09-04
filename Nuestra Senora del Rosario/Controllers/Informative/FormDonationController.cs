using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.DTOS.CreatesDto;
using Services.Informative.FormDonationService;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FormDonationController : ControllerBase
{
    private readonly ISvFormDonation _formDonationService;
    private readonly IMapper _mapper;

    public FormDonationController(ISvFormDonation formDonationService, IMapper mapper)
    {
        _formDonationService = formDonationService;
        _mapper = mapper;
    }

    // GET: api/FormDonation
    [HttpGet]
    public async Task<IActionResult> GetFormDonations()
    {
        var formDonations = await _formDonationService.GetFormDonationsWithDetailsAsync();
        var formDonationDtos = _mapper.Map<IEnumerable<FormDonationDto>>(formDonations);
        return Ok(formDonationDtos);
    }

    // POST: api/FormDonation
    [HttpPost]
    public async Task<IActionResult> AddFormDonation(FormDonationCreateDto formDonationCreateDto)
    {
        var formDonation = _mapper.Map<FormDonation>(formDonationCreateDto);
        await _formDonationService.AddAsync(formDonation);
        await _formDonationService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFormDonation), new { id = formDonation.Id_FormDonation }, formDonation);
    }

    // GET: api/FormDonation/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormDonation(int id)
    {
        var formDonation = await _formDonationService.GetByIdAsync(id);
        if (formDonation == null)
        {
            return NotFound();
        }

        var formDonationDto = _mapper.Map<FormDonationDto>(formDonation);
        return Ok(formDonationDto);
    }

    // PATCH: api/FormDonation/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchFormDonation(int id, [FromBody] JsonPatchDocument<FormDonation> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _formDonationService.PatchAsync(id, patchDoc);
        await _formDonationService.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/FormDonation/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormDonation(int id)
    {
        await _formDonationService.DeleteAsync(id);
        await _formDonationService.SaveChangesAsync();
        return NoContent();
    }
}
