using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Persistence.AppDbContext; // Ajusta al namespace de tu contexto

namespace Infrastructure.Services.Informative.ApplicationFormService
{
    public class SvApplicationForm : ISvApplicationForm
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SvApplicationForm(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<ApplicationFormDto> Forms, int TotalPages)> GetAllFormsAsync(int pageNumber, int pageSize)
        {
            // Contar total de formularios
            var totalForms = await _context.ApplicationForms.CountAsync();

            // Obtener la lista paginada
            var forms = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.ApplicationStatus)  // Sólo si quieres traer el estado
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(af => _mapper.Map<ApplicationFormDto>(af))
                .ToListAsync();

            // Calcular total de páginas
            var totalPages = (int)Math.Ceiling(totalForms / (double)pageSize);
            return (forms, totalPages);
        }

        public async Task<ApplicationFormDto> GetFormByIdAsync(int id)
        {
            // Buscar el formulario con su estado
            var applicationForm = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.ApplicationStatus)
                .FirstOrDefaultAsync(af => af.Id_ApplicationForm == id);

            if (applicationForm == null) return null;

            // Mapear a DTO
            return _mapper.Map<ApplicationFormDto>(applicationForm);
        }

        public async Task<int> AddFormAsync(ApplicationFormCreateDto formCreateDto)
        {
            // Mapeamos a la entidad ApplicationForm
            var applicationForm = _mapper.Map<ApplicationForm>(formCreateDto);

            // Ajustamos campos extra (fecha, estado, etc.)
            applicationForm.ApplicationDate = DateTime.UtcNow;
            applicationForm.Id_Status = 1; // Ejemplo: Pendiente

            // Guardar en la BD
            await _context.ApplicationForms.AddAsync(applicationForm);
            await _context.SaveChangesAsync();

            // Retornar el nuevo ID
            return applicationForm.Id_ApplicationForm;
        }


        public async Task DeleteAsync(int id)
        {
            // Borrado simple por ID
            var form = new ApplicationForm { Id_ApplicationForm = id };
            _context.ApplicationForms.Remove(form);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFormStatusAsync(int id, int statusId)
        {
            var applicationForm = await _context.ApplicationForms.FindAsync(id);
            if (applicationForm == null)
                throw new KeyNotFoundException($"Formulario de aplicación con ID {id} no encontrado.");

            // Verificar que el estado existe
            var statusExists = await _context.ApplicationStatuses.AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
                throw new ArgumentException("El estado proporcionado no existe.");

            applicationForm.Id_Status = statusId;
            await _context.SaveChangesAsync();
        }
    }
}
