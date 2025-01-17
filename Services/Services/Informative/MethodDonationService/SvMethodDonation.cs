using Domain.Entities.Informative;
using Infrastructure.Persistence.MyDbContextInformative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.MethodDonationService
{
    public class SvMethodDonation : SvGenericRepository<MethodDonation, MyInformativeContext>, ISvMethodDonation
    {
        public SvMethodDonation(MyInformativeContext context) : base(context)
        {
        }

        // Método especializado para obtener métodos de donación con sus tipos
        public async Task<IEnumerable<MethodDonation>> GetMethodDonationsWithTypesAsync()
        {
            return await _context.MethodDonations
                .Include(md => md.DonationType)  // Incluir la relación con DonationType
                .ToListAsync();
        }
    }
}