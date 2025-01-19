using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Importa tu AppDbContext
using Infrastructure.Persistence.AppDbContext;

namespace Services.GenericService
{
    public class SvGenericRepository<T> : ISvGenericRepository<T>
        where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public SvGenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
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

        // Este método asume que la entidad T tiene una columna "Dni_Employee" de tipo int
        // Si no lo usas, puedes eliminarlo.
        public async Task<T> GetByDniAsync(int dniEmployee)
        {
            // EF.Property<int> para acceder a la propiedad por nombre
            return await _dbSet.FirstOrDefaultAsync(u => EF.Property<int>(u, "Dni_Employee") == dniEmployee);
        }

        public async Task<bool> ExistsAsync(Func<T, bool> predicate)
        {
            // Como no tenemos un from-linq con EF, se simula con .Any() en memoria
            // O, si tu predicado usa EF, necesitarías cambiar la firma.
            return await Task.FromResult(_dbSet.Any(predicate));
        }

        public async Task<T> FirstOrDefaultAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.FirstOrDefault(predicate));
        }
    }
}
