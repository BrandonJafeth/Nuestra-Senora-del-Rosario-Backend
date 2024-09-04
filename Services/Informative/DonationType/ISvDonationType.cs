using Services.Informative.GenericRepository;
using System;
using Entities.Informative;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.DonationType
{
    public interface ISvDonationType : ISvGenericRepository<Entities.Informative.DonationType>
    {
        // Método específico para obtener tipos de donación con sus métodos
        Task<IEnumerable<Entities.Informative.DonationType>> GetDonationTypesWithMethodsAsync();
    }
}
