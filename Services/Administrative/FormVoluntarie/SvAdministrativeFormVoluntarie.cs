
using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Administrative.FormVoluntarieService
{
    public class AdministrativeFormVoluntarieService : SvGenericRepository<FormVoluntarie>, IAdministrativeFormVoluntarieService
    {
        private readonly AdministrativeContext _context;

        public AdministrativeFormVoluntarieService(AdministrativeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormVoluntarie>> GetAllFormVoluntariesWithTypeAsync()
        {
            return await _context.FormVoluntaries
                .Include(f => f.VoluntarieType)  // Incluir el tipo de voluntariado
                .ToListAsync();
        }

        public async Task<FormVoluntarie> GetFormVoluntarieWithTypeByIdAsync(int id)
        {
            return await _context.FormVoluntaries
                .Include(f => f.VoluntarieType)  // Incluir el tipo de voluntariado
                .FirstOrDefaultAsync(f => f.Id_FormVoluntarie == id);
        }

    }
}
