using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Administrative.MedicationSpecifics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.ResidentPathologies;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentPathologyController : ControllerBase
    {
        private readonly ISvResidentPathology _residentPathologyService;

        public ResidentPathologyController(ISvResidentPathology residentPathologyService)
        {
            _residentPathologyService = residentPathologyService;
        }

        // GET: api/ResidentPathology
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pathologies = await _residentPathologyService.GetAllAsync();
            return Ok(pathologies);
        }

        // GET: api/ResidentPathology/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pathology = await _residentPathologyService.GetByIdAsync(id);
                return Ok(pathology);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/ResidentPathology
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResidentPathologyCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDto = await _residentPathologyService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id_ResidentPathology }, createdDto);
        }

        // PUT: api/ResidentPathology/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResidentPathologyUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _residentPathologyService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/ResidentPathology/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _residentPathologyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
