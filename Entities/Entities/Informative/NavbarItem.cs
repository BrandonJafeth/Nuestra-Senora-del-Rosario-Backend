using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Informative
{
    public class NavbarItem
    {
        public int Id_Nav_It { get; set; }
        public string Title_Nav { get; set; }
        public string UrlNav { get; set; }
        public int Order_Item_Nav { get; set; }
        public bool IsActive { get; set; } = true;

        // Propiedad de navegación para la relación jerárquica (auto-referencia)
        public int? ParentId { get; set; }
        public NavbarItem Parent { get; set; }
        public ICollection<NavbarItem> Children { get; set; }
    }
}
