using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Persistence.AppDbContext;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Infrastructure.Services.Administrative.Residents
{
    public class SvResident : ISvResident
    {
        private readonly ISvGenericRepository<Resident> _residentRepository;
        private readonly ISvGenericRepository<Guardian> _guardianRepository;
        private readonly ISvGenericRepository<Room> _roomRepository;
        private readonly ISvGenericRepository<DependencyHistory> _dependencyHistoryRepository;
        private readonly ISvGenericRepository<DependencyLevel> _dependencyLevelRepository; // Repositorio de DependencyLevel
        private readonly AppDbContext _context; // Inyecta el contexto de base de datos
        private readonly IMapper _mapper;
        private readonly IValidator<ResidentCreateDto> _residentCreateValidator;
        private readonly IValidator<ResidentFromApplicantDto> _residentFromApplicantValidator;

        public SvResident(
            ISvGenericRepository<Resident> residentRepository,
            ISvGenericRepository<Guardian> guardianRepository,
            ISvGenericRepository<Room> roomRepository,
            ISvGenericRepository<DependencyHistory> dependencyHistoryRepository,
            ISvGenericRepository<DependencyLevel> dependencyLevelRepository, // Inyección del repositorio de DependencyLevel
            AppDbContext context, // Añade el contexto aquí
            IMapper mapper, IValidator<ResidentCreateDto> residentCreateValidator, 
        IValidator<ResidentFromApplicantDto> residentFromApplicantValidator) 
        {
            _residentRepository = residentRepository;
            _guardianRepository = guardianRepository;
            _roomRepository = roomRepository;
            _dependencyHistoryRepository = dependencyHistoryRepository;
            _dependencyLevelRepository = dependencyLevelRepository; // Inyección del repositorio de DependencyLevel
            _context = context; // Asigna el contexto
            _mapper = mapper;
            _residentCreateValidator = residentCreateValidator;
            _residentFromApplicantValidator = residentFromApplicantValidator;
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
            // Validar el DTO usando FluentValidation
            var validationResult = await _residentCreateValidator.ValidateAsync(residentDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Validation failed: {errors}");
            }

            var guardian = await _guardianRepository.GetByIdAsync(residentDto.Id_Guardian);
            if (guardian == null)
                throw new KeyNotFoundException($"Guardian with ID {residentDto.Id_Guardian} not found");

            var room = await _roomRepository.GetByIdAsync(residentDto.Id_Room);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {residentDto.Id_Room} not found");

            var dependencyLevel = await _dependencyLevelRepository.GetByIdAsync(residentDto.Id_DependencyLevel);
            if (dependencyLevel == null)
                throw new KeyNotFoundException($"DependencyLevel with ID {residentDto.Id_DependencyLevel} not found");

            var resident = _mapper.Map<Resident>(residentDto);
            await _residentRepository.AddAsync(resident);
            await _residentRepository.SaveChangesAsync();

            var dependencyHistory = new DependencyHistory
            {
                Id_Resident = resident.Id_Resident,
                Id_DependencyLevel = residentDto.Id_DependencyLevel
            };

            await _dependencyHistoryRepository.AddAsync(dependencyHistory);
            await _dependencyHistoryRepository.SaveChangesAsync();
        }



        //// Método para añadir un residente desde un Applicant aprobado
        public async Task AddResidentFromApplicantAsync(ResidentFromApplicantDto dto)
        {
            // Validar el DTO usando FluentValidation
            var validationResult = await _residentFromApplicantValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Validation failed: {errors}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var applicationForm = await _context.ApplicationForms
                    .FirstOrDefaultAsync(af => af.Id_ApplicationForm == dto.Id_ApplicationForm);
                if (applicationForm == null)
                    throw new KeyNotFoundException($"Application Form with ID {dto.Id_ApplicationForm} not found");

                var guardian = await _guardianRepository.Query()
                    .FirstOrDefaultAsync(g => g.Cedula_GD == applicationForm.GuardianCedula);
                if (guardian == null)
                {
                    guardian = new Guardian
                    {
                        Name_GD = applicationForm.GuardianName,
                        Lastname1_GD = applicationForm.GuardianLastName1,
                        Lastname2_GD = applicationForm.GuardianLastName2,
                        Cedula_GD = applicationForm.GuardianCedula,
                        Email_GD = applicationForm.GuardianEmail,
                        Phone_GD = applicationForm.GuardianPhone
                    };
                    await _guardianRepository.AddAsync(guardian);
                    await _guardianRepository.SaveChangesAsync();
                }

                var room = await _roomRepository.GetByIdAsync(dto.Id_Room);
                if (room == null)
                    throw new KeyNotFoundException($"Room with ID {dto.Id_Room} not found");

                var resident = _mapper.Map<Resident>(dto);
                resident.Name_RD = applicationForm.Name_AP;
                resident.Lastname1_RD = applicationForm.LastName1_AP;
                resident.Lastname2_RD = applicationForm.LastName2_AP;
                resident.Cedula_RD = applicationForm.Cedula_AP;
                resident.Id_Guardian = guardian.Id_Guardian;
                resident.Location_RD = applicationForm.Location_AP;
                resident.Status = "Activo";

                await _residentRepository.AddAsync(resident);
                await _residentRepository.SaveChangesAsync();

                var dependencyHistory = new DependencyHistory
                {
                    Id_Resident = resident.Id_Resident,
                    Id_DependencyLevel = dto.Id_DependencyLevel
                };

                await _dependencyHistoryRepository.AddAsync(dependencyHistory);
                await _dependencyHistoryRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("An error occurred while adding the resident from applicant.", ex);
            }
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
