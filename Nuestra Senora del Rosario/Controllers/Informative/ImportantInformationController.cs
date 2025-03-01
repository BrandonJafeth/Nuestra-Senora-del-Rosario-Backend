using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class ImportantInformationController : ControllerBase
{
    private readonly ISvGenericRepository<ImportantInformation> _importantInformationService;
    private readonly IMapper _mapper;

    public ImportantInformationController(ISvGenericRepository<ImportantInformation> importantInformationService, IMapper mapper)
    {
        _importantInformationService = importantInformationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetImportantInformation()
    {
        var items = await _importantInformationService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImportantInfo(int id)
    {
        var item = await _importantInformationService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddImportantInfo(ImportantInformation importantInformation)
    {
        await _importantInformationService.AddAsync(importantInformation);
        await _importantInformationService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetImportantInfo), new { id = importantInformation.Id_ImportantInformation }, importantInformation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateImportanInformation(int id, [FromBody] ImportantInformationUpdateDTO updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _importantInformationService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"DonationsSection con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _importantInformationService.SaveChangesAsync();
        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImportantInfo(int id)
    {
        await _importantInformationService.DeleteAsync(id);
        await _importantInformationService.SaveChangesAsync();
        return NoContent();
    }
}
