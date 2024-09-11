using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class DonationsSectionController : ControllerBase
{
    private readonly ISvGenericRepository<DonationsSection> _donationsSectionService;

    public DonationsSectionController(ISvGenericRepository<DonationsSection> donationsSectionService)
    {
        _donationsSectionService = donationsSectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDonationsSections()
    {
        var items = await _donationsSectionService.GetAllAsync();
        return Ok(items);
    }

   

    

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDonationsSection(int id, [FromBody] JsonPatchDocument<DonationsSection> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _donationsSectionService.PatchAsync(id, patchDoc);
        await _donationsSectionService.SaveChangesAsync();

        return NoContent();
    }

   
}
