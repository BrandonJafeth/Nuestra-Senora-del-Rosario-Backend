using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Administrative.FormVoluntarieService
{
    public class AdministrativeFormVoluntarieService : SvGenericRepository<FormVoluntarie, AdministrativeContext>, IAdministrativeFormVoluntarieService
    {
        // Constructor que pasa el contexto a la clase base
        public AdministrativeFormVoluntarieService(AdministrativeContext context) : base(context)
        {
        }

        // Método para obtener todas las formas de voluntarios con su tipo
        public async Task<IEnumerable<FormVoluntarie>> GetAllFormVoluntariesWithTypeAsync()
        {
            return await _context.FormVoluntaries
                .Include(f => f.VoluntarieType)  // Incluir el tipo de voluntariado
                .ToListAsync();
        }

        // Método para obtener una forma de voluntario con su tipo por ID
        public async Task<FormVoluntarie> GetFormVoluntarieWithTypeByIdAsync(int id)
        {
            return await _context.FormVoluntaries
                .Include(f => f.VoluntarieType)  // Incluir el tipo de voluntariado
                .FirstOrDefaultAsync(f => f.Id_FormVoluntarie == id);
        }
    }
}
