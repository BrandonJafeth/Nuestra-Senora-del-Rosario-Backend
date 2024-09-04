using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.Informative.GenericRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FormDonationController : ControllerBase
{
    private readonly ISvGenericRepository<FormDonation> _formDonationService;
    private readonly IMapper _mapper;

    public FormDonationController(ISvGenericRepository<FormDonation> formDonationService, IMapper mapper)
    {
        _formDonationService = formDonationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetFormDonations()
    {
        var formDonations = await _formDonationService.GetAllAsync();
        var formDonationDtos = _mapper.Map<IEnumerable<FormDonationDto>>(formDonations);
        return Ok(formDonationDtos);
    }

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

    [HttpPost]
    public async Task<IActionResult> AddFormDonation(FormDonationDto formDonationDto)
    {
        var formDonation = _mapper.Map<FormDonation>(formDonationDto);
        await _formDonationService.AddAsync(formDonation);
        await _formDonationService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetFormDonation), new { id = formDonation.Id_FormDonation }, formDonationDto);
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormDonation(int id)
    {
        await _formDonationService.DeleteAsync(id);
        await _formDonationService.SaveChangesAsync();
        return NoContent();
    }
}
