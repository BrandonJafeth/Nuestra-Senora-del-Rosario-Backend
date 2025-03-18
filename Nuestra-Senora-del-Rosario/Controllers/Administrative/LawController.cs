using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using AutoMapper;
using Services.GenericService; // Asumiendo aquí está tu repositorio genérico

[ApiController]
[Route("api/[controller]")]
public class LawController : ControllerBase
{
    private readonly ISvGenericRepository<Law> _lawService;
    private readonly IMapper _mapper;

    public LawController(ISvGenericRepository<Law> lawService, IMapper mapper)
    {
        _lawService = lawService;
        _mapper = mapper;
    }

    // GET: api/law
    [HttpGet]
    public async Task<IActionResult> GetAllLaws([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (laws, totalRecords) = await _lawService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderBy(l => l.LawName)
        );

        var lawDtos = _mapper.Map<IEnumerable<LawReadDto>>(laws);

        var response = new
        {
            Data = lawDtos,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
    }


    // GET: api/law/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLawById(int id)
    {
        var law = await _lawService.GetByIdAsync(id);
        if (law == null)
            return NotFound($"Law with ID {id} not found.");

        var dto = _mapper.Map<LawReadDto>(law);
        return Ok(dto);
    }

    // POST: api/law
    [HttpPost]
    public async Task<IActionResult> CreateLaw([FromBody] LawCreateDto lawCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var lawEntity = _mapper.Map<Law>(lawCreateDto);
        await _lawService.AddAsync(lawEntity);
        await _lawService.SaveChangesAsync();

        var readDto = _mapper.Map<LawReadDto>(lawEntity);
        return CreatedAtAction(nameof(GetLawById), new { id = readDto.IdLaw }, readDto);
    }

    // PUT: api/law/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLaw(int id, [FromBody] LawCreateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _lawService.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Law with ID {id} not found.");

        _mapper.Map(updateDto, existing);  // actualiza LawName, LawDescription
        await _lawService.SaveChangesAsync();

        var readDto = _mapper.Map<LawReadDto>(existing);
        return Ok(readDto);
    }

    // DELETE: api/law/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLaw(int id)
    {
        var existing = await _lawService.GetByIdAsync(id);
        if (existing == null)
            return NotFound($"Law with ID {id} not found.");

        await _lawService.DeleteAsync(id);
        await _lawService.SaveChangesAsync();
        return NoContent();
    }
}
