using Entities.Informative;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.MethodDonationService
{
    public interface ISvMethodDonation : ISvGenericRepository<MethodDonation>
    {
        // Método específico para obtener métodos de donación con su tipo de donación
        Task<IEnumerable<MethodDonation>> GetMethodDonationsWithTypesAsync();
    }
}
