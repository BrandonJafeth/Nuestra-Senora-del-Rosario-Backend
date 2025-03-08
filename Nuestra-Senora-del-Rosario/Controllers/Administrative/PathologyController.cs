using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;  
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;     

using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathologyController : ControllerBase
    {
        private readonly ISvGenericRepository<Pathology> _pathologyService;
        private readonly IMapper _mapper;

        public PathologyController(ISvGenericRepository<Pathology> pathologyService, IMapper mapper)
        {
            _pathologyService = pathologyService;
            _mapper = mapper;
        }

        // GET: api/Pathology
        [HttpGet]
        public async Task<IActionResult> GetAllPathologies()
        {
            var pathologies = await _pathologyService.GetAllAsync();
            var pathologyDtos = _mapper.Map<IEnumerable<PathologyGetDto>>(pathologies);
            return Ok(pathologyDtos);
        }

        // GET: api/Pathology/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPathologyById(int id)
        {
            var pathology = await _pathologyService.GetByIdAsync(id);
            if (pathology == null)
            {
                return NotFound($"Pathology with ID {id} not found.");
            }
            var pathologyDto = _mapper.Map<PathologyGetDto>(pathology);
            return Ok(pathologyDto);
        }

        // POST: api/Pathology
        [HttpPost]
        public async Task<IActionResult> CreatePathology([FromBody] PathologyCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pathology = _mapper.Map<Pathology>(createDto);
            await _pathologyService.AddAsync(pathology);
            await _pathologyService.SaveChangesAsync();

            var pathologyDto = _mapper.Map<PathologyGetDto>(pathology);
            return CreatedAtAction(nameof(GetPathologyById), new { id = pathologyDto.Id_Pathology }, pathologyDto);
        }

        // PUT: api/Pathology/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePathology(int id, [FromBody] PathologyUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pathology = await _pathologyService.GetByIdAsync(id);
            if (pathology == null)
            {
                return NotFound($"Pathology with ID {id} not found.");
            }

            // Mapear los cambios del DTO sobre la entidad existente
            _mapper.Map(updateDto, pathology);
            await _pathologyService.SaveChangesAsync();

            var pathologyDto = _mapper.Map<PathologyGetDto>(pathology);
            return Ok(pathologyDto);
        }

        // DELETE: api/Pathology/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePathology(int id)
        {
            var pathology = await _pathologyService.GetByIdAsync(id);
            if (pathology == null)
            {
                return NotFound($"Pathology with ID {id} not found.");
            }

            await _pathologyService.DeleteAsync(id);
            await _pathologyService.SaveChangesAsync();
            return NoContent();
        }
    }
}
