using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class ConversionParametersDto
    {
        // Factor para conversión simple (ej. litros por caja)
        public double? Factor { get; set; }
        // Unidades que conforman un paquete (ej. 20 unidades)
        public int? UnitsPerPack { get; set; }
        // Paquetes que conforman una caja (ej. 4 paquetes por caja)
        public int? PacksPerBox { get; set; }
    }
}
