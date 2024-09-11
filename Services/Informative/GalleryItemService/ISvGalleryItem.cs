using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Informative;
using Services.GenericService;

namespace Services.Informative.GalleryItemServices
{
    public interface ISvGalleryItem : ISvGenericRepository<GalleryItem>
    {
        
        Task<IEnumerable<GalleryItem>> GetItemsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    }
}
