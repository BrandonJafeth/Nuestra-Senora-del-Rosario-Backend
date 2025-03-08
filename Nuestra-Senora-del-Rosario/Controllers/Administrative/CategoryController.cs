using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Domain.Entities.Administration;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ISvGenericRepository<Category> _categoryService;
    private readonly IMapper _mapper;

    public CategoryController(ISvGenericRepository<Category> categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    // GET: api/category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryGetDTO>>(categories);
        return Ok(categoryDtos);
    }

    // GET: api/category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }
        var categoryDto = _mapper.Map<CategoryGetDTO>(category);
        return Ok(categoryDto);
    }

    // POST: api/category
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var category = _mapper.Map<Category>(categoryCreateDto);
        await _categoryService.AddAsync(category);
        await _categoryService.SaveChangesAsync();

        var categoryDto = _mapper.Map<CategoryGetDTO>(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryID }, categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _categoryService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Category con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _categoryService.SaveChangesAsync();

        return Ok(existingSection);
    }


    // DELETE: api/category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        await _categoryService.DeleteAsync(id);
        await _categoryService.SaveChangesAsync();
        return NoContent();
    }
}
