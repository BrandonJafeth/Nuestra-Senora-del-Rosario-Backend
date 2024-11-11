using Microsoft.AspNetCore.Mvc;
using Services.Informative.DTOS;
using Services.Informative.DTOS.CreatesDto;
using Services.Informative.FormDonationService;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

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
    public async Task<IActionResult> GetFormDonationsWithDetails(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _formDonationService.GetFormDonationsWithDetailsAsync(pageNumber, pageSize);

        return Ok(new
        {
            donations = result.Donations,
            totalPages = result.TotalPages
        });
    }


    // GET: api/FormDonation/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFormDonation(int id)
    {
        var formDonation = await _formDonationService.GetFormDonationWithDetailsByIdAsync(id);
        if (formDonation == null)
        {
            return NotFound();
        }

        return Ok(formDonation);
    }

    // POST: api/FormDonation
    [HttpPost]
    public async Task<IActionResult> CreateFormDonation([FromBody] FormDonationCreateDto formDonationCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _formDonationService.CreateFormDonationAsync(formDonationCreateDto);
            return StatusCode(201); // Devuelve código 201 Created
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // Si ocurre error de validación, se devuelve un BadRequest
        }
    }

    // PATCH: api/FormDonation/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateFormDonationStatus(int id, [FromBody] int statusId)
    {
        try
        {
            await _formDonationService.UpdateFormDonationStatusAsync(id, statusId);
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




    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonation(int id)
    {
        await _formDonationService.DeleteAsync(id);
        await _formDonationService.SaveChangesAsync();
        return NoContent();
    }
}
