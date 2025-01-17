using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.GenericService;

namespace Infrastructure.Services.Informative.DonationType
{
    public interface ISvDonationType : ISvGenericRepository<Domain.Entities.Informative.DonationType>
    {
        // Método específico para obtener tipos de donación con sus métodos
        Task<IEnumerable<Domain.Entities.Informative.DonationType>> GetDonationTypesWithMethodsAsync();
    }
}
