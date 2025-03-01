using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
    public class AssociatesSectionUpdateDto
    {
        [Required]
        public string Subtitle_About_Us { get; set; }

        [Required]
        public string MissionTitle_About_US { get; set; }

        [Required]
        public string MissionDescription_About_US { get; set; }

        [Required]
        public string VisionTitle_About_US { get; set; }

        [Required]
        public string VisionDescription_About_US { get; set; }
    }
}
