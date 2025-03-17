using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class DependencyLevelController : ControllerBase
{
    private readonly ISvGenericRepository<DependencyLevel> _dependencyLevelService;
    private readonly IMapper _mapper;

    public DependencyLevelController(ISvGenericRepository<DependencyLevel> dependencyLevelService, IMapper mapper)
    {
        _dependencyLevelService = dependencyLevelService;
        _mapper = mapper;
    }

    // GET: api/dependencylevel
    [HttpGet]
    public async Task<IActionResult> GetAllDependencyLevels([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (dependencyLevels, totalRecords) = await _dependencyLevelService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderBy(d => d.LevelName)
        );

        var response = new
        {
            Data = dependencyLevels,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
    }


    // GET: api/dependencylevel/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDependencyLevelById(int id)
    {
        var dependencyLevel = await _dependencyLevelService.GetByIdAsync(id);
        if (dependencyLevel == null)
        {
            return NotFound($"Dependency Level with ID {id} not found.");
        }
        return Ok(dependencyLevel);
    }

    // POST: api/dependencylevel
    [HttpPost]
    public async Task<IActionResult> CreateDependencyLevel([FromBody] DependencyLevel dependencyLevel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _dependencyLevelService.AddAsync(dependencyLevel);
        await _dependencyLevelService.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDependencyLevelById), new { id = dependencyLevel.Id_DependencyLevel }, dependencyLevel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDependencyLevel(int id, [FromBody] DependencyLevelUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _dependencyLevelService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"DependencyLevel con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _dependencyLevelService.SaveChangesAsync();
        return Ok(existingSection);
    }

    // DELETE: api/dependencylevel/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDependencyLevel(int id)
    {
        await _dependencyLevelService.DeleteAsync(id);
        await _dependencyLevelService.SaveChangesAsync();
        return NoContent();
    }
}
