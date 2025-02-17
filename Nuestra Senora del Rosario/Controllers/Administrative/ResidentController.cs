using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.Residents;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentsController : ControllerBase
    {
        private readonly ISvResident _residentService;

        public ResidentsController(ISvResident residentService)
        {
            _residentService = residentService;
        }

        // GET: api/Residents
        [HttpGet]
        public async Task<IActionResult> GetAllResidents(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _residentService.GetAllResidentsAsync(pageNumber, pageSize);

            return Ok(new
            {
                residents = result.Residents,
                totalPages = result.TotalPages
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllResidentsWithoutPagination()
        {
            var residents = await _residentService.GetAllResidentsAsync();
            return Ok(residents);
        }



        // GET: api/Residents/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResidentById(int id)
        {
            try
            {
                var resident = await _residentService.GetResidentByIdAsync(id);
                return Ok(resident);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Residents
        [HttpPost]
        public async Task<IActionResult> AddResident([FromBody] ResidentCreateDto residentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _residentService.AddResidentAsync(residentDto);
            return CreatedAtAction(nameof(GetResidentById), new { id = residentDto.Cedula_RD }, residentDto);
        }

        // POST: api/Residents/fromApplicant
        [HttpPost("fromApplicant")]
        public async Task<IActionResult> AddResidentFromApplicant([FromBody] ResidentFromApplicantDto residentFromApplicantDto)
        {
            // Verificar si el modelo es válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Llamar al servicio para añadir el residente desde Applicant
                await _residentService.AddResidentFromApplicantAsync(residentFromApplicantDto);

                // Retornar respuesta creada exitosamente
                return CreatedAtAction(nameof(GetResidentById), new { id = residentFromApplicantDto.Id_ApplicationForm }, residentFromApplicantDto);
            }
            catch (KeyNotFoundException ex)
            {
                // Retornar error 404 si no se encuentra algún dato
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Manejar errores inesperados
                return StatusCode(500, new { error = "Ocurrió un error al procesar la solicitud.", details = ex.Message });
            }
        }


        // PUT: api/Residents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResident(int id, [FromBody] ResidentCreateDto residentDto)
        {
            try
            {
                await _residentService.UpdateResidentAsync(id, residentDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Residents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResident(int id)
        {
            try
            {
                await _residentService.DeleteResidentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        // PATCH: api/Residents/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchResident(int id, [FromBody] ResidentPatchDto patchDto)
        {
            try
            {
                await _residentService.PatchResidentAsync(id, patchDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}