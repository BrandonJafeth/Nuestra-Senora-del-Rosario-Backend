using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GenericService
{
    public interface ISvGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task PatchAsync(int id, JsonPatchDocument<T> patchDoc);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        IQueryable<T> Query();  // Este método devuelve un IQueryable para poder hacer "Include
    }
}