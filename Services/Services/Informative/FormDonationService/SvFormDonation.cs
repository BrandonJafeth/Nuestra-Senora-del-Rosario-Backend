using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Persistence.AppDbContext;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Services.Informative.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentValidation;
using iText.Commons.Actions.Contexts;
using Services.GenericService;

namespace Infrastructure.Services.Informative.FormDonationService
{
    public class SvFormDonationService : SvGenericRepository<FormDonation>, ISvFormDonation
    {
        private readonly IMapper _mapper;
        private readonly IValidator<FormDonationCreateDto> _validator;
        private readonly ILogger<SvFormDonationService> _logger;

        public SvFormDonationService(
            AppDbContext context,
            IMapper mapper,
            IValidator<FormDonationCreateDto> validator,
            ILogger<SvFormDonationService> logger) : base(context)
        {
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        // Obtener todas las donaciones con sus detalles y paginación.
        public async Task<(IEnumerable<FormDonationDto> Donations, int TotalPages)> GetFormDonationsWithDetailsAsync(int pageNumber, int pageSize)
        {
            var totalDonations = await _context.FormDonations
            .AsNoTracking()
                .CountAsync();

            var donations = await _context.FormDonations
                .AsNoTracking()
                .Include(fd => fd.DonationType)
                .Include(fd => fd.MethodDonation)
                .Include(fd => fd.Status)
                .OrderBy(fd => fd.Id_FormDonation) // Aseguramos un orden consistente
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fd => _mapper.Map<FormDonationDto>(fd))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalDonations / (double)pageSize);
            return (donations, totalPages);
        }

        // Obtener una donación por ID con detalles.
        public async Task<FormDonationDto> GetFormDonationWithDetailsByIdAsync(int id)
        {
            var donation = await _context.FormDonations
                .AsNoTracking()
                .Include(fd => fd.DonationType)
                .Include(fd => fd.MethodDonation)
                .Include(fd => fd.Status)
                .FirstOrDefaultAsync(fd => fd.Id_FormDonation == id);

            if (donation == null)
            {
                _logger.LogWarning("Formulario de donación con ID {Id} no encontrado.", id);
                throw new KeyNotFoundException($"Formulario de donación con ID {id} no encontrado.");
            }

            return _mapper.Map<FormDonationDto>(donation);
        }

        // Crear una nueva donación, con validación y mapeo.
        public async Task CreateFormDonationAsync(FormDonationCreateDto formDonationCreateDto)
        {
            // Validación del DTO con FluentValidation.
            var validationResult = await _validator.ValidateAsync(formDonationCreateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Fallo en la validación para crear formulario de donación: {Errors}", errors);
                throw new ArgumentException($"Errores de validación: {errors}");
            }

            // Mapeo del DTO a la entidad.
            var donationEntity = _mapper.Map<FormDonation>(formDonationCreateDto);
            donationEntity.Id_Status = 1; // Estado por defecto "Pendiente"

            await _context.FormDonations.AddAsync(donationEntity);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Creado formulario de donación con ID {Id}", donationEntity.Id_FormDonation);
        }

        // Actualizar solo el estado de una donación.
        public async Task UpdateFormDonationStatusAsync(int id, int statusId)
        {
            var donationEntity = await _context.FormDonations.FindAsync(id);
            if (donationEntity == null)
            {
                _logger.LogWarning("No se encontró el formulario de donación con ID {Id} para actualización de estado.", id);
                throw new KeyNotFoundException($"Formulario de donación con ID {id} no encontrado.");
            }

            // Verificar que el estado exista.
            var statusExists = await _context.Statuses
                .AsNoTracking()
                .AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
            {
                _logger.LogWarning("El estado {StatusId} proporcionado no existe.", statusId);
                throw new ArgumentException("El estado proporcionado no existe.");
            }

            donationEntity.Id_Status = statusId;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Actualizado el estado del formulario de donación con ID {Id} a {StatusId}", id, statusId);
        }
    }
}
