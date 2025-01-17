using Entities.Informative;
using Services.GenericService;




namespace Infrastructure.Services.Informative.NavbarItemServices
{
    public interface ISvNavbarItemService : ISvGenericRepository<NavbarItem>
    {

        Task<IEnumerable<NavbarItem>> GetAllWithChildrenAsync();
        Task<NavbarItem> GetNavbarItemWithChildrenAsync(int id);
    }
}
