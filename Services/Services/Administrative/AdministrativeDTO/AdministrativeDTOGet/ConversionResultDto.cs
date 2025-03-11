using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ConversionResultDto
    {
        // Cantidad de cajas resultante de la conversión
        public int Boxes { get; set; }
        // Cantidad de paquetes resultante (si aplica)
        public int Packs { get; set; }
        // Cantidad de unidades restantes (si aplica)
        public int Units { get; set; }
    }
}
