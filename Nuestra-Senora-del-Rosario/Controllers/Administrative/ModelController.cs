using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.Administration;          // Aquí está la entidad Model
using Services.GenericService;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

[ApiController]
[Route("api/[controller]")]
public class ModelController : ControllerBase
{
    private readonly ISvGenericRepository<Model> _modelService;
    private readonly IMapper _mapper;

    public ModelController(ISvGenericRepository<Model> modelService, IMapper mapper)
    {
        _modelService = modelService;
        _mapper = mapper;
    }

    // GET: api/model
    [HttpGet]
    public async Task<IActionResult> GetAllModels()
    {
        var models = await _modelService.GetAllIncludingAsync(m => m.Brand);

        var modelDtos = _mapper.Map<IEnumerable<ModelReadDto>>(models);
        return Ok(modelDtos);
    }

    // GET: api/model/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetModelById(int id)
    {
        var model = await _modelService.GetSingleIncludingAsync(
            x => x.IdModel == id, 
            m => m.Brand          
        );

        if (model == null)
        {
            return NotFound($"Model with ID {id} not found.");
        }

        var modelDto = _mapper.Map<ModelReadDto>(model);
        return Ok(modelDto);
    }

    // POST: api/model
    [HttpPost]
    public async Task<IActionResult> CreateModel([FromBody] ModelCreateDto modelCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newModel = _mapper.Map<Model>(modelCreateDto);
        await _modelService.AddAsync(newModel);
        await _modelService.SaveChangesAsync();

        var modelDto = _mapper.Map<ModelReadDto>(newModel);
        return CreatedAtAction(nameof(GetModelById), new { id = newModel.IdModel }, modelDto);
    }

    // PUT: api/model/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateModel(int id, [FromBody] ModelCreateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingModel = await _modelService.GetByIdAsync(id);
        if (existingModel == null)
        {
            return NotFound($"Model with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingModel);
        await _modelService.SaveChangesAsync();

        return Ok(existingModel);
    }

    // DELETE: api/model/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModel(int id)
    {
        var model = await _modelService.GetByIdAsync(id);
        if (model == null)
        {
            return NotFound($"Model with ID {id} not found.");
        }

        await _modelService.DeleteAsync(id);
        await _modelService.SaveChangesAsync();
        return NoContent();
    }
}
