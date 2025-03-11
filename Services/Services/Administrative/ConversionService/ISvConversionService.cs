using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.ConversionService
{
    public interface ISvConversionService
    {
        // Convierte litros de leche en cajas, considerando un número de litros por caja (por defecto 12)
        ConversionResultDto ConvertMilk(double liters, int litersPerBox = 12);

        // Convierte la cantidad total de unidades (por ejemplo, pañales) en cajas, paquetes y unidades,
        // considerando que cada paquete tiene un número de unidades y cada caja agrupa varios paquetes.
        ConversionResultDto ConvertDiapers(int totalUnits, int unitsPerPack = 20, int packsPerBox = 4);

        // Método genérico de conversión que utiliza un DTO de parámetros.
        ConversionResultDto Convert(double quantity, ConversionParametersDto parameters);

        // Convierte una cantidad en gramos a kilogramos.
        double ConvertGramsToKilograms(int grams);
    }
}
