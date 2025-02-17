using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.GalleryItemService;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GalleryItemController : ControllerBase
{
    private readonly ISvGalleryItem _galleryItemService;
    private readonly IMapper _mapper;

    public GalleryItemController(ISvGalleryItem galleryItemService, IMapper mapper)
    {
        _galleryItemService = galleryItemService;
        _mapper = mapper;
    }

    // GET: api/GalleryItem
    [HttpGet]
    public async Task<IActionResult> GetGalleryItems()
    {
        var items = await _galleryItemService.GetAllAsync();
        var itemsDto = _mapper.Map<IEnumerable<GalleryItemDto>>(items);
        return Ok(itemsDto);
    }

    // GET: api/GalleryItem/ByCategory/{categoryId}?pageNumber=1&pageSize=10
    [HttpGet("ByCategory/{categoryId}")]
    public async Task<IActionResult> GetGalleryItemsByCategory(int categoryId, int pageNumber = 1, int pageSize = 10)
    {
        var items = await _galleryItemService.GetItemsByCategoryAsync(categoryId, pageNumber, pageSize);
        var itemsDto = _mapper.Map<IEnumerable<GalleryItemDto>>(items);
        return Ok(itemsDto);
    }

    // GET: api/GalleryItem/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryItem(int id)
    {
        var item = await _galleryItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var itemDto = _mapper.Map<GalleryItemDto>(item);
        return Ok(itemDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddGalleryItem(GalleryItemDto galleryItemDto)
    {
        // Mapea el DTO a la entidad
        var galleryItem = _mapper.Map<GalleryItem>(galleryItemDto);

        // Agrega el nuevo item usando el servicio
        await _galleryItemService.AddAsync(galleryItem);
        await _galleryItemService.SaveChangesAsync();

        // Mapea de nuevo la entidad creada al DTO para la respuesta
        var itemDto = _mapper.Map<GalleryItemDto>(galleryItem);

        return CreatedAtAction(nameof(GetGalleryItem), new { id = itemDto.Id_GalleryItem }, itemDto);
    }

    // PATCH: api/GalleryItem/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchGalleryItem(int id, [FromBody] JsonPatchDocument<GalleryItem> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _galleryItemService.PatchAsync(id, patchDoc);
        await _galleryItemService.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/GalleryItem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGalleryItem(int id)
    {
        await _galleryItemService.DeleteAsync(id);
        await _galleryItemService.SaveChangesAsync();
        return NoContent();
    }
}
