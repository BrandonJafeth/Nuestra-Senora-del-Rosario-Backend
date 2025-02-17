using AutoMapper;
using Infrastructure.Services.Administrative.FormVoluntarieService;
using Infrastructure.Services.Informative.DTOS;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministrativeFormVoluntarieController : ControllerBase
    {
        private readonly IAdministrativeFormVoluntarieService _repository;
        private readonly IMapper _mapper;

        // Constructor que inyecta el repositorio genérico administrativo y el mapeador
        public AdministrativeFormVoluntarieController(IAdministrativeFormVoluntarieService repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFormVoluntaries()
        {
            var formVoluntaries = await _repository.GetAllFormVoluntariesWithTypeAsync();
            var formVoluntarieDtos = _mapper.Map<IEnumerable<FormVoluntarieDto>>(formVoluntaries);
            return Ok(formVoluntarieDtos);
        }


        // GET: api/AdministrativeFormVoluntarie/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FormVoluntarie>> GetFormVoluntarieWithTypeByIdAsync(int id)
        {
            var formVoluntarie = await _repository.GetByIdAsync(id);
            if (formVoluntarie == null)
            {
                return NotFound();
            }

            return Ok(formVoluntarie); // Si no mapeas a DTO, retorna la entidad directamente
        }

        // PATCH: api/AdministrativeFormVoluntarie/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchFormVoluntarie(int id, [FromBody] JsonPatchDocument<FormVoluntarie> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var formVoluntarie = await _repository.GetByIdAsync(id);
            if (formVoluntarie == null)
            {
                return NotFound();
            }

            await _repository.PatchAsync(id, patchDoc);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/AdministrativeFormVoluntarie/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormVoluntarie(int id)
        {
            var formVoluntarie = await _repository.GetByIdAsync(id);
            if (formVoluntarie == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

    }
}
