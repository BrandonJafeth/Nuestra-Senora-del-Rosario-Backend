using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Persistence.AppDbContext;
using Domain.Entities.Informative;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Informative.FormVoluntarieService
{
    public class SvFormVoluntarieService : SvGenericRepository<FormVoluntarie>, ISvFormVoluntarieService
    {
        private readonly IMapper _mapper;
        private readonly IValidator<FormVoluntarieCreateDto> _validator;
        private readonly ILogger<SvFormVoluntarieService> _logger;

        public SvFormVoluntarieService(AppDbContext context,
                                       IMapper mapper,
                                       IValidator<FormVoluntarieCreateDto> validator,
                                       ILogger<SvFormVoluntarieService> logger)
            : base(context)
        {
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        // Obtener todas las solicitudes de voluntariado con su tipo y estado
        public async Task<(IEnumerable<FormVoluntarieDto> FormVoluntaries, int TotalPages)> GetAllFormVoluntariesWithTypeAsync(int pageNumber, int pageSize)
        {
            var totalFormVoluntaries = await _context.FormVoluntaries.AsNoTracking().CountAsync();

            var formVoluntaries = await _context.FormVoluntaries
                .AsNoTracking()
                .Include(f => f.VoluntarieType)
                .Include(f => f.Status)
                .OrderBy(f => f.Id_FormVoluntarie)  // Para un orden consistente
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => _mapper.Map<FormVoluntarieDto>(f))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalFormVoluntaries / (double)pageSize);
            return (formVoluntaries, totalPages);
        }

        // Obtener una solicitud de voluntariado por ID con su tipo y estado
        public async Task<FormVoluntarieDto> GetFormVoluntarieWithTypeByIdAsync(int id)
        {
            var formVoluntarie = await _context.FormVoluntaries
                .AsNoTracking()
                .Include(f => f.VoluntarieType)
                .Include(f => f.Status)
                .FirstOrDefaultAsync(f => f.Id_FormVoluntarie == id);

            if (formVoluntarie == null)
            {
                _logger.LogWarning("Formulario de voluntario con ID {Id} no encontrado.", id);
                throw new KeyNotFoundException($"Formulario de voluntario con ID {id} no encontrado.");
            }

            return _mapper.Map<FormVoluntarieDto>(formVoluntarie);
        }

        // Crear una nueva solicitud de voluntariado con validación y mapeo
        public async Task CreateFormVoluntarieAsync(FormVoluntarieCreateDto formVoluntarieCreateDto)
        {
            // Validar el DTO mediante FluentValidation
            var validationResult = await _validator.ValidateAsync(formVoluntarieCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Fallo en la validación para crear formulario de voluntariado: {Errors}", errors);
                throw new ArgumentException($"Errores de validación: {errors}");
            }

            // Verificar si el tipo de voluntariado existe
            var voluntarieTypeExists = await _context.VoluntarieTypes
                .AnyAsync(v => v.Id_VoluntarieType == formVoluntarieCreateDto.Id_VoluntarieType);
            if (!voluntarieTypeExists)
            {
                _logger.LogWarning("El tipo de voluntariado con ID {TypeId} no existe.", formVoluntarieCreateDto.Id_VoluntarieType);
                throw new ArgumentException("El tipo de voluntariado proporcionado no existe.");
            }

            // Mapear el DTO a la entidad utilizando AutoMapper
            var formVoluntarie = _mapper.Map<FormVoluntarie>(formVoluntarieCreateDto);
            formVoluntarie.Id_Status = 1; // Estado por defecto (por ejemplo, "Pendiente")

            await _context.FormVoluntaries.AddAsync(formVoluntarie);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Creado formulario de voluntario con ID {Id}", formVoluntarie.Id_FormVoluntarie);
        }

        // Actualizar solo el estado de un formulario de voluntariado
        public async Task UpdateFormVoluntarieStatusAsync(int id, int statusId)
        {
            var formVoluntarie = await _context.FormVoluntaries.FindAsync(id);
            if (formVoluntarie == null)
            {
                _logger.LogWarning("Formulario de voluntario con ID {Id} no encontrado para actualización de estado.", id);
                throw new KeyNotFoundException($"Formulario de voluntario con ID {id} no encontrado.");
            }

            // Verificar si el estado proporcionado existe
            var statusExists = await _context.Statuses.AsNoTracking().AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
            {
                _logger.LogWarning("El estado {StatusId} proporcionado no existe.", statusId);
                throw new ArgumentException("El estado proporcionado no existe.");
            }

            formVoluntarie.Id_Status = statusId;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Actualizado el estado del formulario de voluntario con ID {Id} a {StatusId}", id, statusId);
        }

        // Eliminar una solicitud de voluntariado
        public async Task DeleteFormVoluntarieAsync(int id)
        {
            var formVoluntarie = await _context.FormVoluntaries.FindAsync(id);
            if (formVoluntarie == null)
            {
                _logger.LogWarning("Formulario de voluntario con ID {Id} no encontrado para eliminación.", id);
                throw new KeyNotFoundException($"Formulario de voluntario con ID {id} no encontrado.");
            }

            _context.FormVoluntaries.Remove(formVoluntarie);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Eliminado formulario de voluntario con ID {Id}", id);
        }
    }
}
