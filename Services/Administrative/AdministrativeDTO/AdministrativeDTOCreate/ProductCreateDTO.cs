using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public int CategoryID { get; set; }        // ID de la categoría del producto
        public int UnitOfMeasureID { get; set; }    // ID de la unidad de medida
        public int InitialQuantity { get; set; }    // Cantidad inicial en inventario
    }

}
