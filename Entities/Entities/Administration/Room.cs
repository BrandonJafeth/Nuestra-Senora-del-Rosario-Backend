﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Administration
{
    public class Room
    {
        [Key]
        public int Id_Room { get; set; }

        [Required, MaxLength(10)]
        public string RoomNumber { get; set; }

        public int AvailableSpots { get; set; }

        [Required]
        public int Capacity { get; set; }  // Capacidad de la habitación
    }
}