using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.ConversionService
{
    public class SvConversionService : ISvConversionService
    {
        public ConversionResultDto ConvertMilk(double liters, int litersPerBox = 12)
        {
            // Se redondea hacia arriba para asegurar que se compre la cantidad completa de cajas
            int boxes = (int)Math.Ceiling(liters / litersPerBox);
            return new ConversionResultDto
            {
                Boxes = boxes
            };
        }

        public ConversionResultDto ConvertDiapers(int totalUnits, int unitsPerPack = 20, int packsPerBox = 4)
        {
            // Calcula la cantidad de unidades que caben en una caja
            int unitsPerBox = unitsPerPack * packsPerBox;
            // Calcula el número de cajas completas
            int boxes = totalUnits / unitsPerBox;
            // Calcula el resto para determinar paquetes y unidades sobrantes
            int remainder = totalUnits % unitsPerBox;
            int packs = remainder / unitsPerPack;
            int units = remainder % unitsPerPack;
            return new ConversionResultDto
            {
                Boxes = boxes,
                Packs = packs,
                Units = units
            };
        }

        public ConversionResultDto Convert(double quantity, ConversionParametersDto parameters)
        {
            if (parameters.UnitsPerPack.HasValue && parameters.PacksPerBox.HasValue)
            {
                // Conversión similar a la de pañales para productos que manejan cajas, paquetes y unidades
                int unitsPerBox = parameters.UnitsPerPack.Value * parameters.PacksPerBox.Value;
                int boxes = (int)(quantity / unitsPerBox);
                int remainder = (int)(quantity % unitsPerBox);
                int packs = remainder / parameters.UnitsPerPack.Value;
                int units = remainder % parameters.UnitsPerPack.Value;
                return new ConversionResultDto
                {
                    Boxes = boxes,
                    Packs = packs,
                    Units = units
                };
            }
            else if (parameters.Factor.HasValue)
            {
                // Conversión simple basada en un factor (por ejemplo, litros por caja)
                int boxes = (int)Math.Ceiling(quantity / parameters.Factor.Value);
                return new ConversionResultDto
                {
                    Boxes = boxes
                };
            }
            else
            {
                throw new ArgumentException("Parámetros insuficientes para la conversión.");
            }
        }


        public double ConvertGramsToKilograms(int grams)
        {
            // Conversión simple: 1 kilogramo = 1000 gramos.
            return grams / 1000.0;
        }
    }
}