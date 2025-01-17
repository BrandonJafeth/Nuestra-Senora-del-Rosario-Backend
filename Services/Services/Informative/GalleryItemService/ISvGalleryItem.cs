using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.Informative;
using Services.GenericService;

namespace Infrastructure.Services.Informative.GalleryItemService
{
    public interface ISvGalleryItem : ISvGenericRepository<GalleryItem>
    {

        Task<IEnumerable<GalleryItem>> GetItemsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    }
}
