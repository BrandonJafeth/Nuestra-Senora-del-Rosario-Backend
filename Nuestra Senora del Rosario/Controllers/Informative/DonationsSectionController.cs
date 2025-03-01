using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class DonationsSectionController : ControllerBase
{
    private readonly ISvGenericRepository<DonationsSection> _donationsSectionService;
    private readonly IMapper _mapper;

    public DonationsSectionController(ISvGenericRepository<DonationsSection> donationsSectionService, IMapper mapper)
    {
        _donationsSectionService = donationsSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetDonationsSections()
    {
        var items = await _donationsSectionService.GetAllAsync();
        return Ok(items);
    }





    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonationsSection(int id, [FromBody] DonationsSectionUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _donationsSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"DonationsSection con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _donationsSectionService.SaveChangesAsync();
        return Ok(existingSection);
    }



}
