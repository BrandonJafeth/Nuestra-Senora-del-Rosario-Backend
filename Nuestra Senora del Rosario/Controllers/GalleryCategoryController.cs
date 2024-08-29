using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GalleryCategoryController : ControllerBase
{
    private readonly ISvGenericRepository<GalleryCategory> _galleryCategoryService;

    public GalleryCategoryController(ISvGenericRepository<GalleryCategory> galleryCategoryService)
    {
        _galleryCategoryService = galleryCategoryService;
    }

    // GET: api/GalleryCategory
    [HttpGet]
    public async Task<IActionResult> GetGalleryCategories()
    {
        var items = await _galleryCategoryService.GetAllAsync();
        return Ok(items);
    }

    // GET: api/GalleryCategory/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryCategory(int id)
    {
        var item = await _galleryCategoryService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    // POST: api/GalleryCategory
    [HttpPost]
    public async Task<IActionResult> AddGalleryCategory(GalleryCategory galleryCategory)
    {
        await _galleryCategoryService.AddAsync(galleryCategory);
        await _galleryCategoryService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGalleryCategory), new { id = galleryCategory.Id_GalleryCategory }, galleryCategory);
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
