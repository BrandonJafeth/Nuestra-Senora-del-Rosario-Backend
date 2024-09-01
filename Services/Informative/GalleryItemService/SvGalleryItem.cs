using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.Informative.GenericRepository;
using Services.MyDbContext;

namespace Services.Informative.GalleryItemServices
{
    public class SvGalleryItem : SvGenericRepository<GalleryItem>, ISvGalleryItem
    {
        private readonly MyContext _context;

        public SvGalleryItem(MyContext context) : base(context)
        {
            _context = context;
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
