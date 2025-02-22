using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.ResidentPathologies
{
    public class SvResidentPathology : ISvResidentPathology
    {
        private readonly ISvGenericRepository<ResidentPathology> _repository;
        private readonly IMapper _mapper;

        public SvResidentPathology(ISvGenericRepository<ResidentPathology> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResidentPathologyGetDto>> GetAllAsync()
        {
            var entities = await _repository.Query()
                .Include(rp => rp.Resident)
                .Include(rp => rp.Pathology)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ResidentPathologyGetDto>>(entities);
        }

        public async Task<ResidentPathologyGetDto> GetByIdAsync(int id)
        {
            var entity = await _repository.Query()
                .Include(rp => rp.Resident)
                .Include(rp => rp.Pathology)
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.Id_ResidentPathology == id);

            if (entity == null)
                throw new KeyNotFoundException($"ResidentPathology with ID {id} not found.");

            return _mapper.Map<ResidentPathologyGetDto>(entity);
        }

        public async Task<ResidentPathologyGetDto> CreateAsync(ResidentPathologyCreateDto dto)
        {
            var entity = _mapper.Map<ResidentPathology>(dto);
            entity.CreatedAt = DateTime.Now;
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ResidentPathologyGetDto>(entity);
        }

        public async Task UpdateAsync(int id, ResidentPathologyUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"ResidentPathology with ID {id} not found.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now;
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"ResidentPathology with ID {id} not found.");

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}
