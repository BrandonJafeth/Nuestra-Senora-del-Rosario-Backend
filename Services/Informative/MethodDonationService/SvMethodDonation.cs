using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.MethodDonationService
{
    public class SvMethodDonation : SvGenericRepository<MethodDonation>, ISvMethodDonation
    {
        private readonly MyInformativeContext _context;

        public SvMethodDonation(MyInformativeContext context) : base(context)
        {
            _context = context;
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