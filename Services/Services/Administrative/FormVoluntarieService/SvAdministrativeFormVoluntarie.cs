using Domain.Entities.Administration;
using Infrastructure.Persistence.AppDbContext; // <-- Ajusta si tu carpeta difiere
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.FormVoluntarieService
{
    public class AdministrativeFormVoluntarieService
        : SvGenericRepository<FormVoluntarie>, IAdministrativeFormVoluntarieService
    {
        // Constructor que pasa el nuevo AppDbContext a la clase base
        public AdministrativeFormVoluntarieService(AppDbContext context) : base(context)
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
