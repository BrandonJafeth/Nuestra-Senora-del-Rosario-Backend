using AutoMapper;
using Entities.Informative;
using Infrastructure.Services.Informative.DonationType;
using Infrastructure.Services.Informative.DTOS;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class DonationTypeController : ControllerBase
{
    private readonly ISvDonationType _donationTypeService;
    private readonly IMapper _mapper;

    public DonationTypeController(ISvDonationType donationTypeService, IMapper mapper)
    {
        _donationTypeService = donationTypeService;
        _mapper = mapper;
    }

    // GET: api/DonationType
    [HttpGet]
    public async Task<IActionResult> GetDonationTypes()
    {
        var donationTypes = await _donationTypeService.GetDonationTypesWithMethodsAsync();
        var donationTypeDtos = _mapper.Map<IEnumerable<DonationTypeDto>>(donationTypes);
        return Ok(donationTypeDtos);
    }

    // GET: api/DonationType/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDonationType(int id)
    {
        var donationType = await _donationTypeService.GetByIdAsync(id);
        if (donationType == null)
        {
            return NotFound();
        }

        var donationTypeDto = _mapper.Map<DonationTypeDto>(donationType);
        return Ok(donationTypeDto);
    }

    // POST: api/DonationType
    [HttpPost]
    public async Task<IActionResult> AddDonationType(DonationTypeDto donationTypeDto)
    {
        var donationType = _mapper.Map<DonationType>(donationTypeDto);
        await _donationTypeService.AddAsync(donationType);
        await _donationTypeService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDonationType), new { id = donationType.Id_DonationType }, donationTypeDto);
    }

    // PATCH: api/DonationType/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDonationType(int id, [FromBody] JsonPatchDocument<DonationType> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _donationTypeService.PatchAsync(id, patchDoc);
        await _donationTypeService.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/DonationType/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonationType(int id)
    {
        await _donationTypeService.DeleteAsync(id);
        await _donationTypeService.SaveChangesAsync();
        return NoContent();
    }
}
