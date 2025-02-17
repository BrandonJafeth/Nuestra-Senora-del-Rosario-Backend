
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Guardians
{
    public class SvGuardian : ISvGuardian
    {
        private readonly ISvGenericRepository<Guardian> _repository;

        public SvGuardian(ISvGenericRepository<Guardian> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Guardian>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Guardian> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Guardian guardian)
        {
            await _repository.AddAsync(guardian);
        }

        public async Task PatchAsync(int id, JsonPatchDocument<Guardian> patchDoc)
        {
            await _repository.PatchAsync(id, patchDoc);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }

        // Lógica personalizada de búsqueda por nombre o apellido
        public async Task<List<Guardian>> SearchGuardiansByNameAsync(string name)
        {
            return await _repository.Query()
                .Where(g => EF.Functions.Like(g.Name_GD, $"%{name}%") ||
                            EF.Functions.Like(g.Lastname1_GD, $"%{name}%") ||
                            EF.Functions.Like(g.Lastname2_GD, $"%{name}%"))
                .ToListAsync();
        }
    }
}
