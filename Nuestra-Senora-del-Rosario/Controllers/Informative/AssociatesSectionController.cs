using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AssociatesSectionController : ControllerBase
{
    private readonly ISvGenericRepository<AssociatesSection> _associatesSectionService;
    private readonly IMapper _mapper;   

    public AssociatesSectionController(ISvGenericRepository<AssociatesSection> associatesSectionService, IMapper mapper)
    {
        _associatesSectionService = associatesSectionService;
        _mapper = mapper;
    }

    // GET: api/AssociatesSection
    [HttpGet]
    public async Task<IActionResult> GetAssociatesSections()
    {
        var items = await _associatesSectionService.GetAllAsync();
        return Ok(items);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssociatesSection(int id, [FromBody] AssociatesSectionUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Obtener la entidad existente de la base de datos (la cual ya está en tracking)
        var existingSection = await _associatesSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"AssociatesSection con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _associatesSectionService.SaveChangesAsync();
        return Ok(existingSection);
    }


}
