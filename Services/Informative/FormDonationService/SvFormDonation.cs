using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.Informative.GenericRepository;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.FormDonationService
{
    public class SvFormDonationService : SvGenericRepository<FormDonation>, ISvFormDonation
    {
        private readonly MyContext _context;

        public SvFormDonationService(MyContext context) : base(context)
        {
            _context = context;
        }

        // Obtener todas las donaciones con sus detalles (DonationType y MethodDonation)
        public async Task<IEnumerable<FormDonation>> GetFormDonationsWithDetailsAsync()
        {
            return await _context.FormDonations
                .Include(fd => fd.DonationType)
                .Include(fd => fd.MethodDonation)
                .ToListAsync();
        }
    }
}
