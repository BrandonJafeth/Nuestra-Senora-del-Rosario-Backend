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

    public TypeOfSalaryController(ISvGenericRepository<TypeOfSalary> typeOfSalaryService)
    {
        _typeOfSalaryService = typeOfSalaryService;
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

    // PATCH: api/typeofsalary/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchTypeOfSalary(int id, [FromBody] JsonPatchDocument<TypeOfSalary> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        try
        {
            // Usa el servicio genérico para aplicar el patch
            await _typeOfSalaryService.PatchAsync(id, patchDoc);

            // Guarda los cambios en el contexto de la base de datos
            await _typeOfSalaryService.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content si fue exitoso
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred while patching TypeOfSalary: {ex.Message}");
        }
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
