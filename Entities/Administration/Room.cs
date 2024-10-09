using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Administration
{
    public class Room
    {
        [Key]
        public int Id_Room { get; set; }

        [Required, MaxLength(10)]
        public string RoomNumber { get; set; }  // Número de la habitación

        [Required]
        public int Capacity { get; set; }  // Capacidad de la habitación
    }
}