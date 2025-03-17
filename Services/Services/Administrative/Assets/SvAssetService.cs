using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Persistence.AppDbContext;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Assets
{
    public class SvAssetService : ISvAssetService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SvAssetService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retorna todos los activos con category y model->brand incluidos,
        /// mapeará a AssetReadDto (así BrandName, CategoryName, etc. no quedan null).
        /// </summary>
        public async Task<IEnumerable<AssetReadDto>> GetAllAsync()
        {
            var assets = await _context.Assets
                .Include(a => a.AssetCategory)
                .Include(a => a.Model)
                    .ThenInclude(m => m.Brand)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AssetReadDto>>(assets);
        }

        /// <summary>
        /// Retorna el Asset por Id con include de categoría, modelo y marca;
        /// lanza excepción si no lo encuentra.
        /// </summary>
        public async Task<AssetReadDto> GetByIdAsync(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.AssetCategory)
                .Include(a => a.Model)
                    .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(a => a.IdAsset == id);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset con ID {id} no encontrado.");
            }

            return _mapper.Map<AssetReadDto>(asset);
        }

        public async Task<AssetReadDto> CreateAsync(AssetCreateDto dto)
        {
            if (dto.PurchaseDate > DateTime.Now)
            {
                throw new ArgumentException("La fecha de compra no puede ser futura.");
            }

            // Mapear DTO -> Entidad
            var assetEntity = _mapper.Map<Asset>(dto);

            // Guardar en DB
            _context.Assets.Add(assetEntity);
            await _context.SaveChangesAsync();

            // Recargar con includes para poder mapear a un AssetReadDto completo
            assetEntity = await _context.Assets
                .Include(a => a.AssetCategory)
                .Include(a => a.Model)
                    .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(a => a.IdAsset == assetEntity.IdAsset);

            return _mapper.Map<AssetReadDto>(assetEntity);
        }

 
        public async Task<AssetReadDto> UpdateAsync(int id, AssetCreateDto dto)
        {
            var existingAsset = await _context.Assets
                .Include(a => a.AssetCategory)
                .Include(a => a.Model)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(a => a.IdAsset == id);

            if (existingAsset == null)
            {
                throw new KeyNotFoundException($"Asset con ID {id} no encontrado.");
            }

            if (dto.PurchaseDate > DateTime.Now)
            {
                throw new ArgumentException("La fecha de compra no puede ser futura.");
            }

            // Mapear del DTO a la entidad existente.
            // Mantiene IdAsset.
            _mapper.Map(dto, existingAsset);

            // Guardar
            await _context.SaveChangesAsync();

            // Volver a recargar para asegurarse de que si cambió IdModel, tengas el nuevo Include
            existingAsset = await _context.Assets
                .Include(a => a.AssetCategory)
                .Include(a => a.Model)
                    .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(a => a.IdAsset == id);

            return _mapper.Map<AssetReadDto>(existingAsset);
        }

        /// <summary>
        /// Elimina un Asset por Id, lanza excepción si no existe.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var existingAsset = await _context.Assets.FindAsync(id);
            if (existingAsset == null)
            {
                throw new KeyNotFoundException($"Asset con ID {id} no encontrado.");
            }

            _context.Assets.Remove(existingAsset);
            await _context.SaveChangesAsync();
        }
    }
}