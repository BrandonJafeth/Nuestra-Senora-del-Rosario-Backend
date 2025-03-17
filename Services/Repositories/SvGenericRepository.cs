using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Importa tu AppDbContext
using Infrastructure.Persistence.AppDbContext;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<T>> GetAllIncludingAsync(
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var includeExpr in includes)
            {
                query = query.Include(includeExpr);
            }

            return await query.ToListAsync();
        }


        public async Task<T> GetSingleIncludingAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var includeExpr in includes)
            {
                query = query.Include(includeExpr);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Iniciamos con la consulta base
            IQueryable<T> query = _dbSet;

            // Aplicamos filtros si existen
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Incluimos las propiedades de navegación
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            // Obtenemos el total de registros que coinciden con los filtros
            int totalCount = await query.CountAsync();

            // Aplicamos ordenación si se especificó
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Aplicamos la paginación
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Ejecutamos la consulta
            var items = await query.ToListAsync();

            // Devolvemos los resultados y el conteo total
            return (items, totalCount);
        }

    }
}
