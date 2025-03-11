using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
   public class ProductGetConvertDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int TotalQuantity { get; set; }
        public string CategoryName { get; set; }
        public string UnitOfMeasure { get; set; }

        // Campos para la conversión
        public double? ConvertedTotalQuantity { get; set; }
        public string ConvertedUnitOfMeasure { get; set; }
    }
}
