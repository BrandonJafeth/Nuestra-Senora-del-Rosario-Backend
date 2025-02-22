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

namespace Infrastructure.Services.Administrative.MedicationSpecifics
{
    public class SvMedicationSpecific : ISvMedicationSpecific
    {
        private readonly ISvGenericRepository<MedicationSpecific> _medSpecRepository;
        private readonly IMapper _mapper;

        public SvMedicationSpecific(
            ISvGenericRepository<MedicationSpecific> medSpecRepository,
            IMapper mapper)
        {
            _medSpecRepository = medSpecRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MedicationSpecificGetDto>> GetAllAsync()
        {
            var entities = await _medSpecRepository.Query()
         .AsNoTracking()
         .Include(ms => ms.UnitOfMeasure)
         .Include(ms => ms.AdministrationRoute)
         .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<MedicationSpecificGetDto>>(entities);
            return dtos;
        }

        public async Task<MedicationSpecificGetDto> GetByIdAsync(int id)
        {
            var entities = await _medSpecRepository.Query()
            .AsNoTracking()
           .Include(ms => ms.UnitOfMeasure)
           .Include(ms => ms.AdministrationRoute)
           .FirstOrDefaultAsync(ms => ms.Id_MedicamentSpecific == id);

            if (entities == null)
                throw new KeyNotFoundException($"MedicationSpecific con ID {id} no encontrado.");

            return _mapper.Map<MedicationSpecificGetDto>(entities);
        }

        public async Task<MedicationSpecificGetDto> CreateAsync(MedicationSpecificCreateDto dto)
        {
            // Mapear DTO → Entidad
            var newEntity = _mapper.Map<MedicationSpecific>(dto);

            // Asignar la fecha de creación (aunque ya lo hace tu propiedad con = DateTime.Now)
            newEntity.CreatedAt = DateTime.Now;
            // UpdatedAt no se asigna de momento (solo cuando se hace update)

            // Guardar
            await _medSpecRepository.AddAsync(newEntity);
            await _medSpecRepository.SaveChangesAsync();

            // Retornar el objeto creado en versión GetDto
            return _mapper.Map<MedicationSpecificGetDto>(newEntity);
        }

        public async Task UpdateAsync(int id, MedicationSpecificUpdateDto dto)
        {
            // Buscar el registro existente
            var existing = await _medSpecRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"MedicationSpecific con ID {id} no encontrado.");

            // Actualizar las propiedades
            _mapper.Map(dto, existing);  // Automapper sobreescribe las propiedades
            existing.UpdatedAt = DateTime.Now;  // Fecha de modificación

            // Guardar cambios
            await _medSpecRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Verificar si existe
            var existing = await _medSpecRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"MedicationSpecific con ID {id} no encontrado.");

            await _medSpecRepository.DeleteAsync(id);
            await _medSpecRepository.SaveChangesAsync();
        }
    }
}
