using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.Administration;          // Aquí está la entidad AssetCategory
using Services.GenericService;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

[ApiController]
[Route("api/[controller]")]
public class AssetCategoryController : ControllerBase
{
    private readonly ISvGenericRepository<AssetCategory> _categoryService;
    private readonly IMapper _mapper;

    public AssetCategoryController(ISvGenericRepository<AssetCategory> categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    // GET: api/assetcategory
    [HttpGet]
    public async Task<IActionResult> GetAllCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (categories, totalRecords) = await _categoryService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderBy(c => c.CategoryName)
        );

        var categoryDtos = _mapper.Map<IEnumerable<AssetCategoryReadDto>>(categories);

        var response = new
        {
            Data = categoryDtos,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
    }


    // GET: api/assetcategory/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"AssetCategory with ID {id} not found.");
        }

        var categoryDto = _mapper.Map<AssetCategoryReadDto>(category);
        return Ok(categoryDto);
    }

    // POST: api/assetcategory
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] AssetCategoryCreateDto categoryCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = _mapper.Map<AssetCategory>(categoryCreateDto);
        await _categoryService.AddAsync(category);
        await _categoryService.SaveChangesAsync();

        var categoryDto = _mapper.Map<AssetCategoryReadDto>(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.IdAssetCategory }, categoryDto);
    }

    // PUT: api/assetcategory/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] AssetCategoryCreateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCategory = await _categoryService.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return NotFound($"AssetCategory with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingCategory);
        await _categoryService.SaveChangesAsync();

        return Ok(existingCategory);
    }

    // DELETE: api/assetcategory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"AssetCategory with ID {id} not found.");
        }

        await _categoryService.DeleteAsync(id);
        await _categoryService.SaveChangesAsync();
        return NoContent();
    }
}
