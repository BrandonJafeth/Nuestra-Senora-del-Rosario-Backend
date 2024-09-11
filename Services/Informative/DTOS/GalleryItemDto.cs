using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.DTOS
{
    public class GalleryItemDto
    {
        public int Id_GalleryItem { get; set; }

        [Required]
        public int Id_GalleryCategory { get; set; }

        [Required]
        [MaxLength(255)]
        public string Gallery_Image_Url { get; set; }
    }
}
