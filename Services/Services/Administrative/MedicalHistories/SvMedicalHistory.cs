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

namespace Infrastructure.Services.Administrative.MedicalHistories
{
    public class SvMedicalHistory : ISvMedicalHistory
    {
        private readonly ISvGenericRepository<MedicalHistory> _medicalHistoryRepository;
        private readonly IMapper _mapper;

        public SvMedicalHistory(ISvGenericRepository<MedicalHistory> medicalHistoryRepository, IMapper mapper)
        {
            _medicalHistoryRepository = medicalHistoryRepository ?? throw new ArgumentNullException(nameof(medicalHistoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Obtiene un historial médico por su ID (lectura optimizada con AsNoTracking)
        public async Task<MedicalHistoryGetDto> GetByIdAsync(int id)
        {
            var entity = await _medicalHistoryRepository.Query()
                .AsNoTracking()
                .Include(mh => mh.Resident)
                .FirstOrDefaultAsync(mh => mh.Id_MedicalHistory == id);

            if (entity == null)
                throw new KeyNotFoundException($"MedicalHistory with ID {id} not found.");

            return _mapper.Map<MedicalHistoryGetDto>(entity);
        }

        // Crea un nuevo historial médico
        public async Task<MedicalHistoryGetDto> CreateAsync(MedicalHistoryCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = _mapper.Map<MedicalHistory>(dto);
            // CreationDate se asigna por defecto en la entidad
            // EditDate se mantiene null en la creación

            await _medicalHistoryRepository.AddAsync(entity);
            await _medicalHistoryRepository.SaveChangesAsync();

            // Se puede reconsultar la entidad con relaciones si fuese necesario.
            return _mapper.Map<MedicalHistoryGetDto>(entity);
        }

        // Actualiza un historial médico existente
        public async Task UpdateAsync(int id, MedicalHistoryUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _medicalHistoryRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"MedicalHistory with ID {id} not found.");

            // Mapear los cambios del DTO a la entidad
            _mapper.Map(dto, entity);
            entity.EditDate = DateTime.Now;

            await _medicalHistoryRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _medicalHistoryRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"MedicalHistory with ID {id} not found.");

            await _medicalHistoryRepository.DeleteAsync(id);
            await _medicalHistoryRepository.SaveChangesAsync();
        }
    }
}
