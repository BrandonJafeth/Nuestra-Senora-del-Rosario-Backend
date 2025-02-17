using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
// Ajusta el namespace si tu AppDbContext está en otra carpeta
using Infrastructure.Persistence.AppDbContext;

namespace Infrastructure.Services.Informative.GalleryItemService
{
    public class SvGalleryItem
        : SvGenericRepository<GalleryItem>, ISvGalleryItem
    {
        public SvGalleryItem(AppDbContext context) : base(context)
        {
        }

        // Método para obtener los GalleryItems por categoría con paginación
        public async Task<IEnumerable<GalleryItem>> GetItemsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            return await _context.GalleryItems
                .Where(gi => gi.Id_GalleryCategory == categoryId)
                .OrderBy(gi => gi.Id_GalleryItem)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
