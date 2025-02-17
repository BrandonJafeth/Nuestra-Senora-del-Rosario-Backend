using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Informative
{
    public class GalleryItem
    {
        public int Id_GalleryItem { get; set; }
        public int Id_GalleryCategory { get; set; }
        public string Gallery_Image_Url { get; set; }

        // Relación con GalleryCategory
        public GalleryCategory GalleryCategory { get; set; }
    }
}
