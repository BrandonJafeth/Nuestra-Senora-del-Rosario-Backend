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

        public async Task<IEnumerable<ApplicationFormDto>> GetAllFormsAsync()
        {
            return await _context.ApplicationForms
                .AsNoTracking() // Desactiva el seguimiento de cambios
                .Include(af => af.Applicant) // Mantén esto solo si realmente necesitas los datos de Applicant
                .Include(af => af.Guardian) // Mantén esto solo si realmente necesitas los datos de Guardian
                .Include(af => af.ApplicationStatus) // Mantén esto solo si realmente necesitas los datos de ApplicationStatus
                .Select(af => _mapper.Map<ApplicationFormDto>(af)) // Usa AutoMapper para mapear a DTO
                .ToListAsync();
        }

        public async Task<ApplicationFormDto> GetFormByIdAsync(int id)
        {
            var applicationForm = await _context.ApplicationForms
                .AsNoTracking()
                .Include(af => af.Applicant)
                .Include(af => af.Guardian)
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
    }
}
