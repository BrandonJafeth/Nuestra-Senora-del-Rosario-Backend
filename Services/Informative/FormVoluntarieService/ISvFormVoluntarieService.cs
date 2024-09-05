using Entities.Informative;
using Services.Informative.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.FormVoluntarieServices
{
    public interface ISvFormVoluntarieService : ISvGenericRepository<FormVoluntarie>
    {
        Task<IEnumerable<FormVoluntarie>> GetFormVoluntariesWithTypesAsync();
    }
}
