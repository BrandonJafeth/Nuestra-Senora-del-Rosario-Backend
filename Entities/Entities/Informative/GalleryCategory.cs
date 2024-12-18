using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Informative
{
    public class GalleryCategory
    {
        public int Id_GalleryCategory { get; set; }
        public string Name_Gallery_Category { get; set; }

        // Relación con GalleryItem
        public ICollection<GalleryItem> GalleryItems { get; set; }
    }
}
