using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Domain.Entities.Administration;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUnitOfMeasure(int id, [FromBody] UnitOfMeasureUpdateDto updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _unitOfMeasureService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"UnitOfMeasure con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _unitOfMeasureService.SaveChangesAsync();
        return Ok(existingSection);
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
