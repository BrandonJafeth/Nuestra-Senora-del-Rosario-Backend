using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Assets
{
    public interface ISvAssetService
    {

        /// <summary>
        /// Retorna todos los Activos con sus relaciones,
        /// mapeados a AssetReadDto (incluye CategoryName, ModelName, BrandName).
        /// </summary>
        Task<IEnumerable<AssetReadDto>> GetAllAsync();

        /// <summary>
        /// Retorna un Activo por Id con sus relaciones, mapeado a AssetReadDto.
        /// </summary>
        Task<AssetReadDto> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo Activo (validando que la fecha no sea futura),
        /// y retorna un AssetReadDto con las relaciones cargadas.
        /// </summary>
        Task<AssetReadDto> CreateAsync(AssetCreateDto dto);

        /// <summary>
        /// Actualiza un Activo existente (validando fecha no futura)
        /// y retorna el AssetReadDto actualizado.
        /// </summary>
        Task<AssetReadDto> UpdateAsync(int id, AssetCreateDto dto);

        /// <summary>
        /// Elimina un Activo por Id.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
