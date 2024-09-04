using Microsoft.EntityFrameworkCore;
using Services.Informative.GenericRepository;
using Services.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.DonationType
{
    public class SvDonationType : SvGenericRepository<Entities.Informative.DonationType>, ISvDonationType
    {
        private readonly MyContext _context;

        public SvDonationType(MyContext context) : base(context)
        {
            _context = context;
        }

        // Método especializado para obtener tipos de donación con sus métodos
        public async Task<IEnumerable<Entities.Informative.DonationType>> GetDonationTypesWithMethodsAsync()
        {
            return await _context.DonationTypes
                .Include(dt => dt.MethodDonations)  // Incluir la relación con MethodDonations
                .ToListAsync();
        }
    }
}
