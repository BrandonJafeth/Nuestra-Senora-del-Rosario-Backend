using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS
{
    public class NavbarItemDto
    {
        public int Id_Nav_It { get; set; }
        public string Title_Nav { get; set; }
        public string UrlNav { get; set; }
        public int Order_Item_Nav { get; set; }
        public bool IsActive { get; set; }
        public ICollection<NavbarItemDto>? Children { get; set; }
    }
}
