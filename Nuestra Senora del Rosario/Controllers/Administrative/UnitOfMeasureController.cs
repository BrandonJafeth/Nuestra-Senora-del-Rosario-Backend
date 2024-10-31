using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Entities.Administration;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

[ApiController]
[Route("api/[controller]")]
public class UnitOfMeasureController : ControllerBase
{
    private readonly ISvGenericRepository<UnitOfMeasure> _unitOfMeasureService;
    private readonly IMapper _mapper;

    public UnitOfMeasureController(ISvGenericRepository<UnitOfMeasure> unitOfMeasureService, IMapper mapper)
    {
        _unitOfMeasureService = unitOfMeasureService;
        _mapper = mapper;
    }

    // GET: api/unitofmeasure
    [HttpGet]
    public async Task<IActionResult> GetAllUnitsOfMeasure()
    {
        var units = await _unitOfMeasureService.GetAllAsync();
        var unitsDto = _mapper.Map<IEnumerable<UnitOfMeasureGetDTO>>(units);
        return Ok(unitsDto);
    }

    // GET: api/unitofmeasure/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUnitOfMeasureById(int id)
    {
        var unit = await _unitOfMeasureService.GetByIdAsync(id);
        if (unit == null)
        {
            return NotFound($"Unit of Measure with ID {id} not found.");
        }
        var unitDto = _mapper.Map<UnitOfMeasureGetDTO>(unit);
        return Ok(unitDto);
    }

    // POST: api/unitofmeasure
    [HttpPost]
    public async Task<IActionResult> CreateUnitOfMeasure([FromBody] UnitOfMeasureCreateDTO createUnitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var unitOfMeasure = _mapper.Map<UnitOfMeasure>(createUnitDto);
        await _unitOfMeasureService.AddAsync(unitOfMeasure);
        await _unitOfMeasureService.SaveChangesAsync();

        var unitDto = _mapper.Map<UnitOfMeasureGetDTO>(unitOfMeasure);
        return CreatedAtAction(nameof(GetUnitOfMeasureById), new { id = unitDto.UnitOfMeasureID }, unitDto);
    }

    // PATCH: api/unitofmeasure/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchUnitOfMeasure(int id, [FromBody] JsonPatchDocument<UnitOfMeasureCreateDTO> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        var unit = await _unitOfMeasureService.GetByIdAsync(id);
        if (unit == null)
        {
            return NotFound($"Unit of Measure with ID {id} not found.");
        }

        // Mapea la entidad a DTO para aplicar el parche
        var unitDto = _mapper.Map<UnitOfMeasureCreateDTO>(unit);
        patchDoc.ApplyTo(unitDto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mapea el DTO modificado de vuelta a la entidad y guarda cambios
        _mapper.Map(unitDto, unit);
        await _unitOfMeasureService.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/unitofmeasure/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUnitOfMeasure(int id)
    {
        await _unitOfMeasureService.DeleteAsync(id);
        await _unitOfMeasureService.SaveChangesAsync();
        return NoContent();
    }
}
