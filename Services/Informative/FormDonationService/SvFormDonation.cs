using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.FormDonationService
{
    public class SvFormDonationService : SvGenericRepository<FormDonation, MyInformativeContext>, ISvFormDonation
    {
        // Constructor que pasa el contexto a la clase base
        public SvFormDonationService(MyInformativeContext context) : base(context)
        {
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
