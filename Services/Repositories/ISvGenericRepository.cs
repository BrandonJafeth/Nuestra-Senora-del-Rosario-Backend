using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;

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
        IQueryable<T> Query();
        Task<T> GetByDniAsync(int dniEmployee);  // Si no lo usas, podrías quitarlo
        Task<bool> ExistsAsync(Func<T, bool> predicate);
        Task<T> FirstOrDefaultAsync(Func<T, bool> predicate);

        Task<IEnumerable<T>> GetAllIncludingAsync(
    params Expression<Func<T, object>>[] includes);

        Task<T> GetSingleIncludingAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
    }
}
