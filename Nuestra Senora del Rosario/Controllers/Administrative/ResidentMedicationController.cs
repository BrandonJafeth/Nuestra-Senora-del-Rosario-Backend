using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Administrative.MedicationSpecifics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.ResidentMedications;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentMedicationController : ControllerBase
    {
        private readonly ISvResidentMedication _residentMedicationService;

        public ResidentMedicationController(ISvResidentMedication residentMedicationService)
        {
            _residentMedicationService = residentMedicationService;
        }

        // GET: api/ResidentMedication
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medications = await _residentMedicationService.GetAllAsync();
            return Ok(medications);
        }

        // GET: api/ResidentMedication/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var medication = await _residentMedicationService.GetByIdAsync(id);
                return Ok(medication);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/ResidentMedication
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResidentMedicationCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDto = await _residentMedicationService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id_ResidentMedication }, createdDto);
        }

        // PUT: api/ResidentMedication/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResidentMedicationUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _residentMedicationService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/ResidentMedication/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _residentMedicationService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
