using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Administrative.ResidentMedications;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;

public class SvResidentMedication : ISvResidentMedication
{
    private readonly ISvGenericRepository<ResidentMedication> _repository;
    private readonly IMapper _mapper;

    public SvResidentMedication(ISvGenericRepository<ResidentMedication> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResidentMedicationGetDto>> GetAllAsync()
    {
      
        var entities = await _repository.Query()
            .Include(rm => rm.Resident)
            .Include(rm => rm.MedicationSpecific)
            .ThenInclude(ms => ms.UnitOfMeasure)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<ResidentMedicationGetDto>>(entities);
    }

    public async Task<ResidentMedicationGetDto> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(rm => rm.Resident)
            .Include(rm => rm.MedicationSpecific)
            .ThenInclude(ms => ms.UnitOfMeasure)
            .AsNoTracking()
            .FirstOrDefaultAsync(rm => rm.Id_ResidentMedication == id);

        if (entity == null)
            throw new KeyNotFoundException($"ResidentMedication with ID {id} not found.");

        return _mapper.Map<ResidentMedicationGetDto>(entity);
    }

    public async Task<ResidentMedicationGetDto> CreateAsync(ResidentMedicationCreateDto dto)
    {
        var entity = _mapper.Map<ResidentMedication>(dto);
        entity.CreatedAt = DateTime.Now;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

    
        return _mapper.Map<ResidentMedicationGetDto>(entity);
    }

    public async Task UpdateAsync(int id, ResidentMedicationUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"ResidentMedication with ID {id} not found.");

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"ResidentMedication with ID {id} not found.");

        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
    }
}
