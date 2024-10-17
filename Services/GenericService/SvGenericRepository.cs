using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GenericService
{
    public class SvGenericRepository<T, TContext> : ISvGenericRepository<T>
        where T : class
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public SvGenericRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
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

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task PatchAsync(int id, JsonPatchDocument<T> patchDoc)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                patchDoc.ApplyTo(entity);
                _context.Entry(entity).State = EntityState.Modified;
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
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByDniAsync(int dniEmployee)
        {
            return await _dbSet.FirstOrDefaultAsync(u => EF.Property<int>(u, "Dni_Employee") == dniEmployee);
        }

        //nuevo codigo
        public async Task<bool> ExistsAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.Any(predicate));
        }

        public async Task<T> FirstOrDefaultAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.FirstOrDefault(predicate));
        }
    }
}