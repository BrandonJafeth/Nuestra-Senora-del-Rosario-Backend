using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.DonationType
{
    public class SvDonationType : SvGenericRepository<DataAccess.Entities.Informative.DonationType, MyInformativeContext>, ISvDonationType
    {
        // Constructor que pasa el contexto a la clase base
        public SvDonationType(MyInformativeContext context) : base(context)
        {
        }

        // Método especializado para obtener tipos de donación con sus métodos
        public async Task<IEnumerable<DataAccess.Entities.Informative.DonationType>> GetDonationTypesWithMethodsAsync()
        {
            return await _context.DonationTypes
                .Include(dt => dt.MethodDonations)  // Incluir la relación con MethodDonations
                .ToListAsync();
        }
    }
}
