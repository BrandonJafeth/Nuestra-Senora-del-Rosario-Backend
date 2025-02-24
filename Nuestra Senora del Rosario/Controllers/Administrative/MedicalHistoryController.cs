using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Administrative.MedicalHistories;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Threading.Tasks;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly ISvMedicalHistory _medicalHistoryService;

        public MedicalHistoryController(ISvMedicalHistory medicalHistoryService)
        {
            _medicalHistoryService = medicalHistoryService;
        }

        // GET: api/MedicalHistory/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalHistoryById(int id)
        {
            try
            {
                var history = await _medicalHistoryService.GetByIdAsync(id);
                return Ok(history);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // GET: api/MedicalHistory/resident/{residentId}
        [HttpGet("resident/{residentId}")]
        public async Task<IActionResult> GetByResidentId(int residentId)
        {
            try
            {
                var histories = await _medicalHistoryService.GetByResidentIdAsync(residentId);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // POST: api/MedicalHistory
        [HttpPost]
        public async Task<IActionResult> CreateMedicalHistory([FromBody] MedicalHistoryCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdHistory = await _medicalHistoryService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetMedicalHistoryById), new { id = createdHistory.Id_MedicalHistory }, createdHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // PUT: api/MedicalHistory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalHistory(int id, [FromBody] MedicalHistoryUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _medicalHistoryService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // DELETE: api/MedicalHistory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalHistory(int id)
        {
            try
            {
                await _medicalHistoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
