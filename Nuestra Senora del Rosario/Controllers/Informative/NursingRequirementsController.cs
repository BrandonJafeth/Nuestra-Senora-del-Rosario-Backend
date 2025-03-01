using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class NursingRequirementsController : ControllerBase
{
    private readonly ISvGenericRepository<NursingRequirements> _nursingRequirementsService;
    private readonly IMapper _mapper;

    public NursingRequirementsController(ISvGenericRepository<NursingRequirements> nursingRequirementsService, IMapper mapper)
    {
        _nursingRequirementsService = nursingRequirementsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetNursingRequirements()
    {
        var items = await _nursingRequirementsService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNursingRequirement(int id)
    {
        var item = await _nursingRequirementsService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddNursingRequirement(NursingRequirements nursingRequirement)
    {
        await _nursingRequirementsService.AddAsync(nursingRequirement);
        await _nursingRequirementsService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNursingRequirement), new { id = nursingRequirement.Id_NursingRequirement }, nursingRequirement);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNursingRequirements(int id, [FromBody] NursingRequirementsUpdateDTO updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _nursingRequirementsService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"NursingRequirements con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _nursingRequirementsService.SaveChangesAsync();

        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNursingRequirement(int id)
    {
        await _nursingRequirementsService.DeleteAsync(id);
        await _nursingRequirementsService.SaveChangesAsync();
        return NoContent();
    }
}
