using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.Guardians
{
    public interface ISvGuardian
    {
        Task<IEnumerable<Guardian>> GetAllAsync();
        Task<Guardian> GetByIdAsync(int id);
        Task AddAsync(Guardian guardian);
        Task PatchAsync(int id, Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<Guardian> patchDoc);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<List<Guardian>> SearchGuardiansByNameAsync(string name); // Nuevo método
    }
}
