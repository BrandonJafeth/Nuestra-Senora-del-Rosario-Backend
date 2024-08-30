using Services.Informative.GenericRepository;
using Entities.Informative;  // Importa el espacio de nombres de las entidades
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.MyDbContext;

namespace Services.Informative.NavbarItemServices
{
    public class SvNavbarItem : SvGenericRepository<NavbarItem>, ISvNavbarItemService
    {
        private readonly MyContext _context;

        public SvNavbarItem(MyContext context) : base(context)
        {
            _context = context;
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
