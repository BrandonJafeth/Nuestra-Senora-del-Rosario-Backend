using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Persistence.AppDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;

namespace Infrastructure.Services.Informative.ApplicationFormService
{
    public class SvApplicationForm : ISvApplicationForm
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<ApplicationFormCreateDto> _validator;

        public SvApplicationForm(AppDbContext context, IMapper mapper, IValidator<ApplicationFormCreateDto> validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<(IEnumerable<ApplicationFormDto> Forms, int TotalPages)> GetAllFormsAsync(int pageNumber, int pageSize)
        {
            var totalForms = await _context.ApplicationForms.CountAsync();

            var forms = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.ApplicationStatus)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(af => _mapper.Map<ApplicationFormDto>(af))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalForms / (double)pageSize);
            return (forms, totalPages);
        }

        public async Task<ApplicationFormDto> GetFormByIdAsync(int id)
        {
            var applicationForm = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.ApplicationStatus)
                .FirstOrDefaultAsync(af => af.Id_ApplicationForm == id);

            if (applicationForm == null) return null;

            return _mapper.Map<ApplicationFormDto>(applicationForm);
        }

        public async Task<int> AddFormAsync(ApplicationFormCreateDto formCreateDto)
        {
            // Validar los datos del DTO
            var validationResult = await _validator.ValidateAsync(formCreateDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Mapeamos a la entidad ApplicationForm
            var applicationForm = _mapper.Map<ApplicationForm>(formCreateDto);

            applicationForm.ApplicationDate = DateTime.UtcNow;
            applicationForm.Id_Status = 1; // Estado inicial


            await _context.ApplicationForms.AddAsync(applicationForm);
            await _context.SaveChangesAsync();

            return applicationForm.Id_ApplicationForm;
        }

        public async Task DeleteAsync(int id)
        {
            var form = new ApplicationForm { Id_ApplicationForm = id };
            _context.ApplicationForms.Remove(form);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFormStatusAsync(int id, int statusId)
        {
            var applicationForm = await _context.ApplicationForms.FindAsync(id);
            if (applicationForm == null)
                throw new KeyNotFoundException($"Formulario de aplicación con ID {id} no encontrado.");

            var statusExists = await _context.ApplicationStatuses.AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
                throw new ArgumentException("El estado proporcionado no existe.");

            applicationForm.Id_Status = statusId;
            await _context.SaveChangesAsync();
        }


        public async Task<ApplicationFormDto> UpdateFormAsync(int id, ApplicationFormUpdateDto updateDto)
        {

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(af => af.Id_ApplicationForm == id);

            if (applicationForm == null)
                throw new KeyNotFoundException($"ApplicationForm con ID {id} no encontrado.");

            _mapper.Map(updateDto, applicationForm);


            await _context.SaveChangesAsync();
            return _mapper.Map<ApplicationFormDto>(applicationForm);
        }
    }
}
