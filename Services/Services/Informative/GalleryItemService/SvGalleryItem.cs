using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Informative;
using Infrastructure.Persistence.MyDbContextInformative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;

namespace Infrastructure.Services.Informative.GalleryItemService
{
    public class SvGalleryItem : SvGenericRepository<GalleryItem, MyInformativeContext>, ISvGalleryItem
    {
        public SvGalleryItem(MyInformativeContext context) : base(context)
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