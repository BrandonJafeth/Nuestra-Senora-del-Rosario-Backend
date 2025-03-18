using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
  public  class RoomReadDto
    {

        public int Id_Room { get; set; }
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }
        public int AvailableSpots { get; set; }
    }
}
