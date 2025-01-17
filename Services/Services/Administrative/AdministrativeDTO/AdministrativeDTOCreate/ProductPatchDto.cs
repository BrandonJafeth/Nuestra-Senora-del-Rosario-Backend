using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ProductPatchDto
    {
        public int? CategoryID { get; set; }
        public int? UnitOfMeasureID { get; set; }
        public int? TotalQuantity { get; set; }
        public string Name { get; set; }
    }
}
