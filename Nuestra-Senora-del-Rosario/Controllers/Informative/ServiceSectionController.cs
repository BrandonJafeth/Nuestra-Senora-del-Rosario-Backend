using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class ServiceSectionController : ControllerBase
{
    private readonly ISvGenericRepository<ServiceSection> _serviceSectionService;
    private readonly IMapper _mapper;

    public ServiceSectionController(ISvGenericRepository<ServiceSection> serviceSectionService, IMapper mapper)
    {
        _serviceSectionService = serviceSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetServiceSections()
    {
        var items = await _serviceSectionService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceSection(int id)
    {
        var item = await _serviceSectionService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddServiceSection(ServiceSection serviceSection)
    {
        await _serviceSectionService.AddAsync(serviceSection);
        await _serviceSectionService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetServiceSection), new { id = serviceSection.Id_ServiceSection }, serviceSection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatServiceSection(int id, [FromBody] ServiceSectionUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _serviceSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"ServiceSection con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _serviceSectionService.SaveChangesAsync();

        return Ok(existingSection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceSection(int id)
    {
        await _serviceSectionService.DeleteAsync(id);
        await _serviceSectionService.SaveChangesAsync();
        return NoContent();
    }
}
