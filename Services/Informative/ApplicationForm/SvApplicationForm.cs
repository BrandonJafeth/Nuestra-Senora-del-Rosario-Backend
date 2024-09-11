using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.MyDbContext;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Informative.DTOS.CreatesDto;

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

        public async Task<IEnumerable<ApplicationForm>> GetAllFormsAsync()
        {
            return await _context.ApplicationForms
                .Include(af => af.Applicant)     // Incluyendo Applicant
                .Include(af => af.Guardian)      // Incluyendo Guardian
                .Include(af => af.ApplicationStatus)  // Incluyendo ApplicationStatus
                .ToListAsync();
        }

        public async Task<ApplicationForm> GetFormByIdAsync(int id)
        {
            return await _context.ApplicationForms
                .Include(af => af.Applicant)  // Incluyendo la relación con Applicant
                .Include(af => af.Guardian)   // Incluyendo la relación con Guardian
                .FirstOrDefaultAsync(af => af.Id_ApplicationForm == id);
        }

        public async Task AddFormAsync(ApplicationFormCreateDto formCreateDto)
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

                // Agregar el guardián antes de agregar el solicitante
                await _context.Guardians.AddAsync(guardian);
                await _context.SaveChangesAsync();  // Guardar para generar el Id_Guardian
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
                    Id_Guardian = guardian.Id_Guardian  // Asignar el Id del guardián existente
                };

                // Agregar el solicitante
                await _context.Applicants.AddAsync(applicant);
            }

            // Crear el nuevo ApplicationForm
            var applicationForm = new ApplicationForm
            {
                Applicant = applicant,
                Guardian = guardian,
                ApplicationDate = DateTime.UtcNow,
                Id_Status = 1  // Estado inicial, por ejemplo, "Pendiente"
            };

            await _context.ApplicationForms.AddAsync(applicationForm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var form = await _context.ApplicationForms.FindAsync(id);
            if (form != null)
            {
                _context.ApplicationForms.Remove(form);
                await _context.SaveChangesAsync();
            }
        }
    }
}
