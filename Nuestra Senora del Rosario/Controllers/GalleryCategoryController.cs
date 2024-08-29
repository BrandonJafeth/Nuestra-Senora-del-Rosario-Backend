using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.Informative.GenericRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    public async Task<IActionResult> AddGalleryCategory(GalleryCategory galleryCategory)
    {
        await _galleryCategoryService.AddAsync(galleryCategory);
        await _galleryCategoryService.SaveChangesAsync();

        var categoryDto = _mapper.Map<GalleryCategoryDto>(galleryCategory);
        return CreatedAtAction(nameof(GetGalleryCategory), new { id = galleryCategory.Id_GalleryCategory }, categoryDto);
    }

    // PATCH: api/GalleryCategory/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchGalleryCategory(int id, [FromBody] JsonPatchDocument<GalleryCategory> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _galleryCategoryService.PatchAsync(id, patchDoc);
        await _galleryCategoryService.SaveChangesAsync();

        return NoContent();
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
