using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class InventoryDailyReportDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int TotalIngresos { get; set; } 
        public int TotalEgresos { get; set; }   
        public string UnitOfMeasure { get; set; }
    }
}
