using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.Assets;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{

    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly ISvAssetService _assetService;

        public AssetController(ISvAssetService assetService)
        {
            _assetService = assetService;
        }

        /// <summary>
        /// GET: api/asset
        /// Retorna todos los assets con sus relaciones (Category, Model, Brand) mapeados a AssetReadDto.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assets = await _assetService.GetAllAsync();
            return Ok(assets);
        }

        /// <summary>
        /// GET: api/asset/{id}
        /// Retorna un asset específico (con categoría y modelo) o 404 si no existe.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var assetDto = await _assetService.GetByIdAsync(id);
                return Ok(assetDto);
            }
            catch (KeyNotFoundException ex)
            {
                // La excepción indica que no encontró el activo
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// POST: api/asset
        /// Crea un nuevo asset, validando que la fecha de compra no sea futura.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssetCreateDto dto)
        {
            // Verifica que las validaciones de modelo se cumplan
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdDto = await _assetService.CreateAsync(dto);
                // Devuelve el 201 Created, junto con la ubicación y el objeto creado
                return CreatedAtAction(nameof(GetById),
                    new { id = createdDto.IdAsset },
                    createdDto);
            }
            catch (ArgumentException ex)
            {
                // Fecha futura u otra validación de negocio
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// PUT: api/asset/{id}
        /// Actualiza un asset existente. Si no existe, 404; si la fecha es futura, 400.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AssetCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDto = await _assetService.UpdateAsync(id, dto);
                return Ok(updatedDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// DELETE: api/asset/{id}
        /// Elimina un asset, o 404 si no se halla.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _assetService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}