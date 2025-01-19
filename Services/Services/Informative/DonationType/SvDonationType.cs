using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Persistence.AppDbContext; // <-- Ajusta si tu AppDbContext está en otro namespace

namespace Infrastructure.Services.Informative.DonationType
{
    public class SvDonationType
        : SvGenericRepository<Domain.Entities.Informative.DonationType>, ISvDonationType
    {
        // Constructor que pasa el nuevo AppDbContext a la clase base
        public SvDonationType(AppDbContext context) : base(context)
        {
        }

        // Método especializado para obtener tipos de donación con sus métodos
        public async Task<IEnumerable<Domain.Entities.Informative.DonationType>> GetDonationTypesWithMethodsAsync()
        {
            return await _context.DonationTypes
                .Include(dt => dt.MethodDonations)  // Incluir la relación con MethodDonations
                .ToListAsync();
        }
    }
}
