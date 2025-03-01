using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Informative.DTOS.CreatesDto
{
    public class RoomUpdateDTO
    {

        [Required, MaxLength(10)]
        public string RoomNumber { get; set; }

        [Required]
        public int Capacity { get; set; } 
    }
}
