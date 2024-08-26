using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.GenericRepository
{
    public class SvGenericRepository<T> : ISvGenericRepository<T> where T : class
    {
        private readonly MyContext _myDbContext;
        private readonly DbSet<T> _dbSet;

        public SvGenericRepository(MyContext myDbContext)
        {
            _myDbContext = myDbContext;
            _dbSet = _myDbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _myDbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task PatchAsync(int id, JsonPatchDocument<T> patchDoc)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                patchDoc.ApplyTo(entity);  
                _myDbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _myDbContext.SaveChangesAsync();
        }
    }
}
