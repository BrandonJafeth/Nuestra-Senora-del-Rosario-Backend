using Entities.Informative;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.FormDonationService
{
    public interface ISvFormDonation : ISvGenericRepository<FormDonation>
    {
        Task<IEnumerable<FormDonation>> GetFormDonationsWithDetailsAsync();
    }
}

