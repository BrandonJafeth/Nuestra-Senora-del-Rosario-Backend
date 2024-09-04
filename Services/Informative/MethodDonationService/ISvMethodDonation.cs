using Entities.Informative;
using Services.Informative.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.MethodDonationService
{
    public interface ISvMethodDonation : ISvGenericRepository<MethodDonation>
    {
        // Método específico para obtener métodos de donación con su tipo de donación
        Task<IEnumerable<MethodDonation>> GetMethodDonationsWithTypesAsync();
    }
}
