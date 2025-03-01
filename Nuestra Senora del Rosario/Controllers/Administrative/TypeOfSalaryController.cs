using AutoMapper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TypeOfSalaryController : ControllerBase
{
    private readonly ISvGenericRepository<TypeOfSalary> _typeOfSalaryService;
    private readonly IMapper _mapper;

    public TypeOfSalaryController(ISvGenericRepository<TypeOfSalary> typeOfSalaryService, IMapper mapper)
    {
        _typeOfSalaryService = typeOfSalaryService;
        _mapper = mapper;
    }

    // GET: api/typeofsalary
    [HttpGet]
    public async Task<IActionResult> GetAllTypesOfSalary()
    {
        var typesOfSalary = await _typeOfSalaryService.GetAllAsync();
        return Ok(typesOfSalary);
    }

    // GET: api/typeofsalary/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeOfSalaryById(int id)
    {
        var typeOfSalary = await _typeOfSalaryService.GetByIdAsync(id);
        if (typeOfSalary == null)
        {
            return NotFound($"Type of Salary with ID {id} not found.");
        }
        return Ok(typeOfSalary);
    }

    // POST: api/typeofsalary
    [HttpPost]
    public async Task<IActionResult> CreateTypeOfSalary([FromBody] TypeOfSalary typeOfSalary)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _typeOfSalaryService.AddAsync(typeOfSalary);
            await _typeOfSalaryService.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTypeOfSalaryById), new { id = typeOfSalary.Id_TypeOfSalary }, typeOfSalary);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeOfSalary(int id, [FromBody] TypeOfSalaryUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _typeOfSalaryService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"TypeOfSalary con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _typeOfSalaryService.SaveChangesAsync();
        return Ok(existingSection);
    }


    // DELETE: api/typeofsalary/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeOfSalary(int id)
    {
        await _typeOfSalaryService.DeleteAsync(id);
        await _typeOfSalaryService.SaveChangesAsync();
        return NoContent();
    }
}
