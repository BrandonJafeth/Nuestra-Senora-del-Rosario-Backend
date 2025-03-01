using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
    public class GalleryItemUpdateDTO
    {

        [Required]
        public int Id_GalleryCategory { get; set; }

        [Required]
        [MaxLength(2000)] 
        public string Gallery_Image_Url { get; set; }
    }
}
