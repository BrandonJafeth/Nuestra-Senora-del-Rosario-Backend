using Domain.Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;
// Ajusta la ruta según dónde tengas tu AppDbContext
using Infrastructure.Persistence.AppDbContext;

namespace Infrastructure.Services.Informative.MethodDonationService
{
    public class SvMethodDonation
        : SvGenericRepository<MethodDonation>, ISvMethodDonation
    {
        public SvMethodDonation(AppDbContext context) : base(context)
        {
        }

        // Método especializado para obtener métodos de donación con sus tipos
        public async Task<IEnumerable<MethodDonation>> GetMethodDonationsWithTypesAsync()
        {
            return await _context.MethodDonations
                .Include(md => md.DonationType)
                .ToListAsync();
        }
    }
}
