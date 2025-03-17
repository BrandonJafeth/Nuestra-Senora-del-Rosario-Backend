using AutoMapper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProfessionController : ControllerBase
{
    private readonly ISvGenericRepository<Profession> _professionService;
    private readonly IMapper _mapper;

    public ProfessionController(ISvGenericRepository<Profession> professionService, IMapper mapper)
    {
        _professionService = professionService;
        _mapper = mapper;
    }

    // POST: api/profession
    [HttpPost]
    public async Task<IActionResult> CreateProfession([FromBody] Profession profession)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
     
            await _professionService.AddAsync(profession);
            await _professionService.SaveChangesAsync();

            return Ok(profession); 
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }


    // GET: api/profession/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfessionById(int id)
    {
        var profession = await _professionService.GetByIdAsync(id);
        if (profession == null)
        {
            return NotFound($"Profession with ID {id} not found.");
        }
        return Ok(profession);
    }

    // GET: api/profession
    [HttpGet]
    public async Task<IActionResult> GetAllProfessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (professions, totalRecords) = await _professionService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize
        );

        var response = new
        {
            Data = professions,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfession(int id, [FromBody] ProfessionUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _professionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Category con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _professionService.SaveChangesAsync();

        return Ok(existingSection);
    }


    // DELETE: api/profession/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfession(int id)
    {
        // Llamamos al método DeleteAsync directamente desde el servicio
        await _professionService.DeleteAsync(id);

        // Guardamos los cambios
        await _professionService.SaveChangesAsync();

        return NoContent(); // Respuesta 204 si todo fue exitoso
    }


}
