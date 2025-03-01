using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class AdministrativeRequirementsController : ControllerBase
{
    private readonly ISvGenericRepository<AdministrativeRequirements> _administrativeRequirementsService;


    private readonly IMapper _mapper;
    public AdministrativeRequirementsController(ISvGenericRepository<AdministrativeRequirements> administrativeRequirementsService,IMapper mapper)
    {
        _administrativeRequirementsService = administrativeRequirementsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAdministrativeRequirements()
    {
        var items = await _administrativeRequirementsService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdministrativeRequirement(int id)
    {
        var item = await _administrativeRequirementsService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddAdministrativeRequirement(AdministrativeRequirements requirement)
    {
        await _administrativeRequirementsService.AddAsync(requirement);
        await _administrativeRequirementsService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAdministrativeRequirement), new { id = requirement.Id_AdministrativeRequirement }, requirement);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdministrariveRequeriments(int id, [FromBody] AdministrariveRequerimentsUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _administrativeRequirementsService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"AdministrariveRequeriment con ID {id} no fue encontrada.");
        }

 
        _mapper.Map(updateDto, existingSection);

        await _administrativeRequirementsService.SaveChangesAsync();

        return Ok(existingSection);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdministrativeRequirement(int id)
    {
        await _administrativeRequirementsService.DeleteAsync(id);
        await _administrativeRequirementsService.SaveChangesAsync();
        return NoContent();
    }
}
