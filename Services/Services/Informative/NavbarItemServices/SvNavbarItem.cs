using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.GenericService;
// Ajusta el using para tu AppDbContext:
using Infrastructure.Persistence.AppDbContext;
using Domain.Entities.Informative;

namespace Infrastructure.Services.Informative.NavbarItemServices
{
    public class SvNavbarItem
        : SvGenericRepository<NavbarItem>, ISvNavbarItemService
    {
        public SvNavbarItem(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NavbarItem>> GetAllWithChildrenAsync()
        {
            return await _context.NavbarItems
                .Where(n => n.ParentId == null)
                .Include(n => n.Children)
                .ToListAsync();
        }

        public async Task<NavbarItem> GetNavbarItemWithChildrenAsync(int id)
        {
            return await _context.NavbarItems
                .Include(n => n.Children)
                .FirstOrDefaultAsync(n => n.Id_Nav_It == id && n.ParentId == null);
        }
    }
}
