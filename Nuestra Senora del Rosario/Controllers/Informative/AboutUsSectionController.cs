using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class AboutUsSectionController : ControllerBase
{
    private readonly ISvGenericRepository<AboutUsSection> _aboutUsSectionService;
    private readonly IMapper _mapper;

    public AboutUsSectionController(ISvGenericRepository<AboutUsSection> aboutUsSectionService,IMapper mapper)
    {
        _aboutUsSectionService = aboutUsSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAboutUsSections()
    {
        var items = await _aboutUsSectionService.GetAllAsync();
        return Ok(items);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAboutUsSection(int id, [FromBody] AssociatesSectionUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Obtener la entidad existente de la base de datos (la cual ya está en tracking)
        var existingSection = await _aboutUsSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"AboutUsSection con ID {id} no fue encontrada.");
        }

        // Mapear los cambios del DTO a la entidad existente
        _mapper.Map(updateDto, existingSection);

        // Guardar cambios (la instancia existingSection ya está rastreada, 
        // por lo que EF Core detecta los cambios automáticamente)
        await _aboutUsSectionService.SaveChangesAsync();

        // Retornar 200 con la entidad actualizada o 204 sin contenido
        // Aquí, por consistencia con el ejemplo de Pathology, retornamos 200 + la entidad actualizada
        return Ok(existingSection);
    }

}
