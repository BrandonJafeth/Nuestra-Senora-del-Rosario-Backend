using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    // DTO para la creación de un movimiento en el inventario
    public class InventoryCreateDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string MovementType { get; set; }
    }
}
