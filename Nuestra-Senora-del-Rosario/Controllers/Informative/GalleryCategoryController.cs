using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class GalleryCategoryController : ControllerBase
{
    private readonly ISvGenericRepository<GalleryCategory> _galleryCategoryService;
    private readonly IMapper _mapper;

    public GalleryCategoryController(ISvGenericRepository<GalleryCategory> galleryCategoryService, IMapper mapper)
    {
        _galleryCategoryService = galleryCategoryService;
        _mapper = mapper;
    }

    // GET: api/GalleryCategory
    [HttpGet]
    public async Task<IActionResult> GetGalleryCategories()
    {
        var categories = await _galleryCategoryService.GetAllAsync();
        var categoryDtos = _mapper.Map<IEnumerable<GalleryCategoryDto>>(categories);
        return Ok(categoryDtos);
    }

    // GET: api/GalleryCategory/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryCategory(int id)
    {
        var category = await _galleryCategoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<GalleryCategoryDto>(category);
        return Ok(categoryDto);
    }

    // POST: api/GalleryCategory
    [HttpPost]
    public async Task<IActionResult> AddGalleryCategory(GalleryCategoryDto galleryCategoryDto)
    {
        // Mapeamos el DTO a la entidad
        var galleryCategory = _mapper.Map<GalleryCategory>(galleryCategoryDto);

        await _galleryCategoryService.AddAsync(galleryCategory);
        await _galleryCategoryService.SaveChangesAsync();

        var createdGalleryCategoryDto = _mapper.Map<GalleryCategoryDto>(galleryCategory);
        return CreatedAtAction(nameof(GetGalleryCategory), new { id = createdGalleryCategoryDto.Id_GalleryCategory }, createdGalleryCategoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGalleryCategory(int id, [FromBody] GalleryCategoryUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _galleryCategoryService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"GalleryCategory con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _galleryCategoryService.SaveChangesAsync();
        return Ok(existingSection);
    }

    // DELETE: api/GalleryCategory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGalleryCategory(int id)
    {
        await _galleryCategoryService.DeleteAsync(id);
        await _galleryCategoryService.SaveChangesAsync();
        return NoContent();
    }
}
