using AutoMapper;
using Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Residents
{
    public class SvResident : ISvResident
    {
        private readonly ISvGenericRepository<Resident> _residentRepository;
        private readonly ISvGenericRepository<Guardian> _guardianRepository;
        private readonly ISvGenericRepository<Room> _roomRepository;
        private readonly ISvGenericRepository<Applicant> _applicantRepository;
        private readonly ISvGenericRepository<DependencyHistory> _dependencyHistoryRepository;
        private readonly ISvGenericRepository<DependencyLevel> _dependencyLevelRepository; // Repositorio de DependencyLevel
        private readonly IMapper _mapper;

        public SvResident(
            ISvGenericRepository<Resident> residentRepository,
            ISvGenericRepository<Guardian> guardianRepository,
            ISvGenericRepository<Room> roomRepository,
            ISvGenericRepository<Applicant> applicantRepository,
            ISvGenericRepository<DependencyHistory> dependencyHistoryRepository,
            ISvGenericRepository<DependencyLevel> dependencyLevelRepository, // Inyección del repositorio de DependencyLevel
            IMapper mapper)
        {
            _residentRepository = residentRepository;
            _guardianRepository = guardianRepository;
            _roomRepository = roomRepository;
            _applicantRepository = applicantRepository;
            _dependencyHistoryRepository = dependencyHistoryRepository;
            _dependencyLevelRepository = dependencyLevelRepository; // Inyección del repositorio de DependencyLevel
            _mapper = mapper;
        }

        // Método para obtener todos los residentes
        public async Task<(IEnumerable<ResidentGetDto> Residents, int TotalPages)> GetAllResidentsAsync(int pageNumber, int pageSize)
        {
            var totalResidents = await _residentRepository.Query().CountAsync();

            var residents = await _residentRepository
                .Query()
                .Include(r => r.Guardian)               // Incluir detalles del guardián
                .Include(r => r.Room)                   // Incluir detalles de la habitación
                .Include(r => r.DependencyHistories)    // Incluir historial de dependencias
                .ThenInclude(dh => dh.DependencyLevel)  // Incluir nivel de dependencia
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalResidents / (double)pageSize);

            return (_mapper.Map<IEnumerable<ResidentGetDto>>(residents), totalPages);
        }

        public async Task<IEnumerable<ResidentGetDto>> GetAllResidentsAsync()
        {
            var residents = await _residentRepository.Query()
                .Include(r => r.Guardian)
                .Include(r => r.Room)
                .Include(r => r.DependencyHistories)
                .ThenInclude(dh => dh.DependencyLevel)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ResidentGetDto>>(residents);
        }



        // Método para obtener un residente por ID
        public async Task<ResidentGetDto> GetResidentByIdAsync(int id)
        {
            var resident = await _residentRepository.Query()
                .Include(r => r.Guardian)  // Incluir los detalles del guardián
                .Include(r => r.Room)  // Incluir los detalles de la habitación
                .Include(r => r.DependencyHistories)  // Incluir el historial de dependencias
                .ThenInclude(dh => dh.DependencyLevel)  // Incluir el nivel de dependencia
                .FirstOrDefaultAsync(r => r.Id_Resident == id);  // Filtrar por ID del residente

            if (resident == null)
                throw new KeyNotFoundException($"Resident with ID {id} not found");

            // Mapear el resultado a ResidentGetDto
            return _mapper.Map<ResidentGetDto>(resident);
        }


        // Método para añadir un residente desde cero
        public async Task AddResidentAsync(ResidentCreateDto residentDto)
        {
            var guardian = await _guardianRepository.GetByIdAsync(residentDto.Id_Guardian);
            if (guardian == null)
                throw new KeyNotFoundException($"Guardian with ID {residentDto.Id_Guardian} not found");

            var room = await _roomRepository.GetByIdAsync(residentDto.Id_Room);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {residentDto.Id_Room} not found");

            var dependencyLevel = await _dependencyLevelRepository.GetByIdAsync(residentDto.Id_DependencyLevel);
            if (dependencyLevel == null)
                throw new KeyNotFoundException($"DependencyLevel with ID {residentDto.Id_DependencyLevel} not found");

            // Mapeo del residente y guardado
            var resident = _mapper.Map<Resident>(residentDto);
            await _residentRepository.AddAsync(resident);
            await _residentRepository.SaveChangesAsync();

            // Crear el historial de dependencia
            var dependencyHistory = new DependencyHistory
            {
                Id_Resident = resident.Id_Resident,
                Id_DependencyLevel = residentDto.Id_DependencyLevel
            };

            await _dependencyHistoryRepository.AddAsync(dependencyHistory);
            await _dependencyHistoryRepository.SaveChangesAsync();
        }



        // Método para añadir un residente desde un Applicant aprobado
        public async Task AddResidentFromApplicantAsync(ResidentFromApplicantDto dto)
        {
            var applicant = await _applicantRepository.GetByIdAsync(dto.Id_Applicant);
            if (applicant == null)
                throw new KeyNotFoundException($"Applicant with ID {dto.Id_Applicant} not found");

            var room = await _roomRepository.GetByIdAsync(dto.Id_Room);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {dto.Id_Room} not found");

            var resident = new Resident
            {
                Name_AP = applicant.Name_AP,
                Lastname1_AP = applicant.Lastname1_AP,
                Lastname2_AP = applicant.Lastname2_AP,
                Cedula_AP = applicant.Cedula_AP,
                Sexo = dto.Sexo,
                FechaNacimiento = DateTime.Now.AddYears(-applicant.Age_AP),
                Id_Guardian = applicant.Id_Guardian,
                Id_Room = dto.Id_Room,
                EntryDate = dto.EntryDate,
                Location = applicant.Location,
                Status = "Activo"
            };

            await _residentRepository.AddAsync(resident);
            await _residentRepository.SaveChangesAsync();

            var dependencyHistory = new DependencyHistory
            {
                Id_Resident = resident.Id_Resident,
                Id_DependencyLevel = dto.Id_DependencyLevel
            };

            await _dependencyHistoryRepository.AddAsync(dependencyHistory);
            await _dependencyHistoryRepository.SaveChangesAsync();
        }

        // Método para actualizar un residente
        public async Task UpdateResidentAsync(int id, ResidentCreateDto residentDto)
        {
            var resident = await _residentRepository.GetByIdAsync(id);
            if (resident == null)
                throw new KeyNotFoundException($"Resident with ID {id} not found");

            _mapper.Map(residentDto, resident); // Actualizar los campos del residente
            _residentRepository.Update(resident);
            await _residentRepository.SaveChangesAsync();
        }

        // Método para eliminar un residente
        public async Task DeleteResidentAsync(int id)
        {
            await _residentRepository.DeleteAsync(id);
            await _residentRepository.SaveChangesAsync();
        }



        public async Task PatchResidentAsync(int id, ResidentPatchDto patchDto)
        {
            // Obtener el residente
            var resident = await _residentRepository.GetByIdAsync(id);
            if (resident == null)
                throw new KeyNotFoundException($"Resident with ID {id} not found");

            // Si se proporciona el Id de la habitación, actualiza la habitación
            if (patchDto.Id_Room.HasValue)
            {
                var room = await _roomRepository.GetByIdAsync(patchDto.Id_Room.Value);
                if (room == null)
                    throw new KeyNotFoundException($"Room with ID {patchDto.Id_Room.Value} not found");

                resident.Id_Room = patchDto.Id_Room.Value;
            }

            // Si se proporciona el estado, actualiza el estado
            if (!string.IsNullOrEmpty(patchDto.Status))
            {
                resident.Status = patchDto.Status;
            }

            // Si se proporciona el Id del nivel de dependencia, actualiza el historial
            if (patchDto.Id_DependencyLevel.HasValue)
            {
                var dependencyLevel = await _dependencyLevelRepository.GetByIdAsync(patchDto.Id_DependencyLevel.Value);
                if (dependencyLevel == null)
                    throw new KeyNotFoundException($"DependencyLevel with ID {patchDto.Id_DependencyLevel.Value} not found");

                // Crear una nueva entrada en el historial de dependencias
                var dependencyHistory = new DependencyHistory
                {
                    Id_Resident = resident.Id_Resident,
                    Id_DependencyLevel = patchDto.Id_DependencyLevel.Value
                };

                await _dependencyHistoryRepository.AddAsync(dependencyHistory);
            }

            // Si se proporciona la fecha de nacimiento, actualiza la fecha de nacimiento
            if (patchDto.FechaNacimiento.HasValue)
            {
                resident.FechaNacimiento = patchDto.FechaNacimiento.Value;
            }

            // Guardar los cambios
            _residentRepository.Update(resident);
            await _residentRepository.SaveChangesAsync();
        }


    }
}
