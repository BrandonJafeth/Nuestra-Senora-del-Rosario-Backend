using Services.Informative.GenericRepository;
using Entities.Informative;




namespace Services.Informative.NavbarItemServices  
{
    public interface ISvNavbarItemService : ISvGenericRepository<NavbarItem>
    {

        Task<IEnumerable<NavbarItem>> GetAllWithChildrenAsync();
        Task<NavbarItem> GetNavbarItemWithChildrenAsync(int id);
    }
}
