using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.MyDbContext;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Informative.DTOS.CreatesDto;
using Services.Informative.DTOS;

namespace Services.Informative.ApplicationFormService
{
    public class SvApplicationForm : ISvApplicationForm
    {
        private readonly MyInformativeContext _context;
        private readonly IMapper _mapper; // Inyección de AutoMapper

        public SvApplicationForm(MyInformativeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<ApplicationFormDto> Forms, int TotalPages)> GetAllFormsAsync(int pageNumber, int pageSize)
        {
            var totalForms = await _context.ApplicationForms.CountAsync(); // Total de formularios

            var forms = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.Applicant)
                .Include(af => af.Guardian)
                .Include(af => af.ApplicationStatus)
                .Skip((pageNumber - 1) * pageSize) // Saltar elementos para la paginación
                .Take(pageSize)                    // Limitar los resultados a pageSize
                .Select(af => _mapper.Map<ApplicationFormDto>(af))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalForms / (double)pageSize);
            return (forms, totalPages);
        }


        public async Task<ApplicationFormDto> GetFormByIdAsync(int id)
        {
            var applicationForm = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.Applicant)
                .Include(af => af.Guardian)
                .Include(af => af.ApplicationStatus)  // Incluir ApplicationStatus
                .FirstOrDefaultAsync(af => af.Id_ApplicationForm == id);

            if (applicationForm == null) return null;

            return _mapper.Map<ApplicationFormDto>(applicationForm);
        }


        public async Task AddFormAsync(ApplicationFormCreateDto formCreateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Verificar si el Guardian ya existe
                var guardian = await _context.Guardians
                    .FirstOrDefaultAsync(g => g.Cedula_GD == formCreateDto.Cedula_GD);

                if (guardian == null)
                {
                    // Crear un nuevo Guardian
                    guardian = new Guardian
                    {
                        Name_GD = formCreateDto.Name_GD,
                        Lastname1_GD = formCreateDto.Lastname1_GD,
                        Lastname2_GD = formCreateDto.Lastname2_GD,
                        Cedula_GD = formCreateDto.Cedula_GD,
                        Phone_GD = formCreateDto.Phone_GD,
                        Email_GD = formCreateDto.Email_GD
                    };

                    await _context.Guardians.AddAsync(guardian);
                    await _context.SaveChangesAsync();
                }

                // Verificar si el Applicant ya existe
                var applicant = await _context.Applicants
                    .FirstOrDefaultAsync(a => a.Cedula_AP == formCreateDto.Cedula_AP);

                if (applicant == null)
                {
                    // Crear un nuevo Applicant
                    applicant = new Applicant
                    {
                        Name_AP = formCreateDto.Name_AP,
                        Lastname1_AP = formCreateDto.Lastname1_AP,
                        Lastname2_AP = formCreateDto.Lastname2_AP,
                        Age_AP = formCreateDto.Age_AP,
                        Cedula_AP = formCreateDto.Cedula_AP,
                        Location = formCreateDto.Location, // Asigna la ubicación desde el DTO
                        Id_Guardian = guardian.Id_Guardian
                    };

                    await _context.Applicants.AddAsync(applicant);
                    await _context.SaveChangesAsync();
                }

                // Crear el nuevo ApplicationForm
                var applicationForm = new ApplicationForm
                {
                    Applicant = applicant,
                    Guardian = guardian,
                    ApplicationDate = DateTime.UtcNow,
                    Id_Status = 1 // Estado inicial, por ejemplo, "Pendiente"
                };

                await _context.ApplicationForms.AddAsync(applicationForm);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var form = new ApplicationForm { Id_ApplicationForm = id };
            _context.ApplicationForms.Remove(form);
            await _context.SaveChangesAsync();
        }


        // Método para actualizar solo el estado de un formulario
        public async Task UpdateFormStatusAsync(int id, int statusId)
        {
            // Buscar el formulario de aplicación por ID
            var applicationForm = await _context.ApplicationForms.FindAsync(id);
            if (applicationForm == null)
            {
                throw new KeyNotFoundException($"Formulario de aplicación con ID {id} no encontrado.");
            }

            // Verificar si el estado proporcionado existe en la tabla ApplicationStatus
            var statusExists = await _context.ApplicationStatuses.AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
            {
                throw new ArgumentException("El estado proporcionado no existe.");
            }

            // Actualizar el estado del formulario
            applicationForm.Id_Status = statusId;
            await _context.SaveChangesAsync();
        }
    }
}
