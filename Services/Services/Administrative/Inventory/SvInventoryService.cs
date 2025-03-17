using AutoMapper;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Administrative.Inventory;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Domain.Entities.Administration;
using iText.Commons.Actions.Contexts;
using Infrastructure.Persistence.AppDbContext;
using FluentValidation;
using Infrastructure.Services.Administrative.ConversionService;

public class SvInventoryService : ISvInventoryService
{
    private readonly ISvGenericRepository<Inventory> _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IValidator<InventoryCreateDTO> _inventoryCreateValidator;
    private readonly ISvConversionService _conversionService;
    public SvInventoryService(ISvGenericRepository<Inventory> inventoryRepository, IMapper mapper, AppDbContext context, IValidator<InventoryCreateDTO> inventoryCreateValidator, ISvConversionService conversionService)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _context = context;
        _inventoryCreateValidator = inventoryCreateValidator;
        _conversionService = conversionService;
    }

    public async Task<IEnumerable<InventoryGetDTO>> GetAllMovementsAsync()
    {
        var movements = await _inventoryRepository
            .Query()
            .Include(m => m.Product)                    // Incluye Product
            .ThenInclude(p => p.Category)               // Incluye Category de Product
            .Include(m => m.Product.UnitOfMeasure)      // Incluye UnitOfMeasure de Product
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryGetDTO>>(movements);
    }

    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAsync(
      int month,
      int year,
      string targetUnit,
      List<int> convertProductIds
  )
    {
        // 1. Obtener todos los productos (o filtrar según tu necesidad real)
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .ToListAsync();

        // 2. Obtener los movimientos del mes
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // 3. Agrupar los movimientos por producto para calcular ingresos y egresos
        var monthlyData = monthlyMovements
            .GroupBy(m => m.ProductID)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
                }
            );

        // 4. Filtrar solo los productos que tuvieron movimiento en el mes
        var productIdsConMovimiento = monthlyMovements
            .Select(m => m.ProductID)
            .Distinct()
            .ToList();

        products = products
            .Where(p => productIdsConMovimiento.Contains(p.ProductID))
            .ToList();

        // 5. Construir el reporte para cada producto
        var report = products.Select(p =>
        {
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;

            // Unidad original
            string originalUnit = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Por defecto, si no hay conversión, dejamos la misma unidad
            string convertedUnit = originalUnit;

            // Obtener ingresos y egresos del mes (si existen)
            int totalIngress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalIngresos
                : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalEgresos
                : 0;

            // Aplica la conversión solo si se especificó targetUnit y está en la lista convertProductIds
            if (!string.IsNullOrEmpty(targetUnit) &&
                convertProductIds != null &&
                convertProductIds.Contains(p.ProductID))
            {
                switch (targetUnit.ToLower())
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        convertedUnit = "Paquete(s)";
                        break;

                    case "kg":
                        // Ejemplo: 1 kg = 1000 g
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        convertedUnit = "kg";
                        break;

                    case "caja":
                        // Si el producto se mide en litros (ej. leche), 1 caja = 12 litros
                        if (originalUnit.ToLower().Contains("litro"))
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            convertedUnit = "Caja(s)";
                        }
                        break;

                    default:
                        // Sin conversión adicional
                        break;
                }
            }

            // Retornar la información final
            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,
                TotalIngresos = totalIngress,
                TotalEgresos = totalEgress,

                // Unidad que se mostrará (podrías dejar la original si así lo deseas)
                UnitOfMeasure = convertedUnit,

                // Cantidad convertida
                ConvertedTotalInStock = convertedQuantity,

                // Nuevas propiedades para mostrar en el JSON
                OriginalUnitOfMeasure = originalUnit,
                ConvertedUnitOfMeasure = convertedUnit
            };
        }).ToList();

        return report;
    }





    public async Task RegisterMovementsAsync(IEnumerable<InventoryCreateDTO> inventoryCreateDTOs)
    {
        if (inventoryCreateDTOs == null || !inventoryCreateDTOs.Any())
        {
            throw new ArgumentException("Debe proporcionar al menos un movimiento de inventario.");
        }

        // Validar cada DTO usando el validador inyectado
        foreach (var dto in inventoryCreateDTOs)
        {
            var result = _inventoryCreateValidator.Validate(dto);
            if (!result.IsValid)
            {
                // Puedes acumular los errores o lanzar la excepción directamente
                throw new ValidationException(result.Errors);
            }
        }

        // Inicia una transacción para asegurar atomicidad
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var dto in inventoryCreateDTOs)
            {
                var inventoryMovement = _mapper.Map<Inventory>(dto);
                await _inventoryRepository.AddAsync(inventoryMovement);
            }

            await _inventoryRepository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task PatchInventoryAsync(int inventoryId, JsonPatchDocument<Inventory> patchDoc)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
        if (inventory == null) throw new KeyNotFoundException($"Inventory record with ID {inventoryId} not found.");

        patchDoc.ApplyTo(inventory);
        await _inventoryRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryGetDTO>> GetMovementsByMonthAsync(int month, int year)
    {
        var movements = await _inventoryRepository
            .Query()
            .Include(m => m.Product)
            .ThenInclude(p => p.Category)
            .Include(m => m.Product.UnitOfMeasure)
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryGetDTO>>(movements);
    }


    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAllProductsAsync(
           int month,
           int year,
           Dictionary<int, string> conversionMapping)
    {
        // 1. Obtener todos los productos
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .ToListAsync();

        // 2. Obtener los movimientos del mes
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // 3. Agrupar movimientos por producto para calcular ingresos y egresos
        var monthlyData = monthlyMovements
            .GroupBy(m => m.ProductID)
            .ToDictionary(g => g.Key, g => new {
                TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
            });

        // 4. Construir el reporte para cada producto
        var report = products.Select(p =>
        {
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;

            // Guardamos la unidad original (lo que viene en p.UnitOfMeasure)
            string originalUnit = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Por defecto, si no se aplica conversión, ambas unidades quedan iguales
            string convertedUnit = originalUnit;

            // Obtener ingresos/egresos
            int totalIngress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalIngresos
                : 0;

            int totalEgress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalEgresos
                : 0;

            // Ver si hay mapping de conversión
            if (conversionMapping != null && conversionMapping.ContainsKey(p.ProductID))
            {
                string targetUnit = conversionMapping[p.ProductID].ToLower();
                switch (targetUnit)
                {
                    case "paquete":
                        convertedQuantity = totalQuantity / 20.0;
                        convertedUnit = "Paquete(s)";
                        break;

                    case "kg":
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        convertedUnit = "kg";
                        break;

                    case "caja":
                        if (p.UnitOfMeasure?.UnitName.ToLower().Contains("litro") == true)
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            convertedUnit = "Caja(s)";
                        }
                        break;

                    default:
                        // Sin conversión adicional
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,
                TotalIngresos = totalIngress,
                TotalEgresos = totalEgress,

                // Antes se usaba UnitOfMeasure para mostrar la unidad (podrías mantenerlo o no)
                UnitOfMeasure = convertedUnit,

                // Cantidad numérica convertida
                ConvertedTotalInStock = convertedQuantity,

                // NUEVAS PROPIEDADES
                OriginalUnitOfMeasure = originalUnit,
                ConvertedUnitOfMeasure = convertedUnit
            };
        }).ToList();

        return report;
    }

    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryAsync(
     int month,
     int year,
     Dictionary<int, string> conversionMapping,
     int categoryId
 )
    {
        // 1. Obtener los productos que pertenecen a la categoría obligatoria
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .Where(p => p.CategoryID == categoryId)
            .ToListAsync();

        // 2. Obtener los movimientos del mes
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // 3. Agrupar movimientos por producto para calcular ingresos y egresos
        var monthlyData = monthlyMovements
            .GroupBy(m => m.ProductID)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
                }
            );

        // 4. Construir el reporte
        var report = products.Select(p =>
        {
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;

            // Unidad original
            string originalUnit = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Unidad "convertida" por defecto
            string convertedUnit = originalUnit;

            // Si el producto tuvo movimientos, sacamos los totales
            int totalIngress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalIngresos
                : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalEgresos
                : 0;

            // 5. Aplicar la conversión si el diccionario contiene una clave para este producto
            if (conversionMapping != null && conversionMapping.ContainsKey(p.ProductID))
            {
                string targetUnit = conversionMapping[p.ProductID].ToLower();
                switch (targetUnit)
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        convertedUnit = "Paquete(s)";
                        break;

                    case "kg":
                        // Ejemplo: para productos en gramos
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        convertedUnit = "kg";
                        break;

                    case "caja":
                        // Si el producto se mide en litros (ej. leche), 1 caja = 12 litros
                        if (originalUnit.ToLower().Contains("litro"))
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            convertedUnit = "Caja(s)";
                        }
                        break;

                    default:
                        // Sin conversión, se deja la unidad original
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,
                TotalIngresos = totalIngress,
                TotalEgresos = totalEgress,

                // Unidad que quieras mostrar por defecto (en este ejemplo, la "convertida")
                UnitOfMeasure = convertedUnit,
                ConvertedTotalInStock = convertedQuantity,

                // NUEVAS PROPIEDADES para mayor claridad
                OriginalUnitOfMeasure = originalUnit,
                ConvertedUnitOfMeasure = convertedUnit
            };
        }).ToList();

        return report;
    }


    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryWithMovementsAsync(
       int month,
       int year,
       Dictionary<int, string> conversionMapping,
       int categoryId
   )
    {
        // 1. Movimientos del mes
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // 2. IDs de producto con movimiento
        var productIds = monthlyMovements
            .Select(m => m.ProductID)
            .Distinct()
            .ToList();

        // 3. Filtrar productos por categoría y movimiento
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .Where(p => p.CategoryID == categoryId && productIds.Contains(p.ProductID))
            .ToListAsync();

        // 4. Agrupar los movimientos para obtener totales de ingresos/egresos
        var monthlyData = monthlyMovements
            .GroupBy(m => m.ProductID)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
                }
            );

        // 5. Construir el reporte
        var report = products.Select(p =>
        {
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;

            // Unidad original
            string originalUnit = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Por defecto, la "convertida" es la misma que la original
            string convertedUnit = originalUnit;

            // Obtener ingresos y egresos del mes
            int totalIngress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalIngresos
                : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID)
                ? monthlyData[p.ProductID].TotalEgresos
                : 0;

            // 6. Aplicar conversión si el diccionario tiene entrada para este producto
            if (conversionMapping != null && conversionMapping.ContainsKey(p.ProductID))
            {
                string targetUnit = conversionMapping[p.ProductID].ToLower();
                switch (targetUnit)
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        convertedUnit = "Paquete(s)";
                        break;

                    case "kg":
                        // Ejemplo: para productos en gramos, 1 kg = 1000 gramos
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        convertedUnit = "kg";
                        break;

                    case "caja":
                        // Si el producto se mide en litros (ej. leche), 1 caja = 12 litros
                        if (originalUnit.ToLower().Contains("litro"))
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            convertedUnit = "Caja(s)";
                        }
                        break;

                    default:
                        // Sin conversión adicional
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,
                TotalIngresos = totalIngress,
                TotalEgresos = totalEgress,

                // Unidad que se está mostrando (la “convertida” o la original, según aplique)
                UnitOfMeasure = convertedUnit,
                ConvertedTotalInStock = convertedQuantity,

                // NUEVAS PROPIEDADES
                OriginalUnitOfMeasure = originalUnit,
                ConvertedUnitOfMeasure = convertedUnit
            };
        }).ToList();

        return report;
    }





    public async Task DeleteInventoryAsync(int inventoryId)
    {
        await _inventoryRepository.DeleteAsync(inventoryId);
        await _inventoryRepository.SaveChangesAsync();
    }



    public async Task<IEnumerable<InventoryDailyReportDTO>> GetDailyMovementsAsync(DateTime date)
    {
        // Filtra movimientos por el día específico y carga las relaciones necesarias
        var movements = await _inventoryRepository.Query()
            .Include(m => m.Product)
            .ThenInclude(p => p.UnitOfMeasure)
            .Where(i => i.Date.Date == date.Date)
            .ToListAsync();

        // Agrupa movimientos por ProductID y calcula el total de Ingresos y Egresos por producto
        var dailyReport = movements
            .GroupBy(m => m.ProductID)
            .Select(g =>
            {
                var firstItem = g.First();
                return new InventoryDailyReportDTO
                {
                    ProductID = firstItem.ProductID,
                    ProductName = firstItem.Product.Name,
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity),
                    UnitOfMeasure = firstItem.Product.UnitOfMeasure.UnitName
                };
            }).ToList();

        return dailyReport;
    }



    public async Task<IEnumerable<InventoryGetDTO>> GetMovementsByDayAsync(int day, int month, int year)
    {
        var movements = await _inventoryRepository
            .Query()
            .Include(m => m.Product)
            .ThenInclude(p => p.Category)
            .Include(m => m.Product.UnitOfMeasure)
            .Where(i => i.Date.Day == day && i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryGetDTO>>(movements);
    }


    public async Task<IEnumerable<InventoryDailyReportDTO>> GetDailyReportAsync(int day, int month, int year)
    {
        var movements = await _inventoryRepository
            .Query()
            .Include(i => i.Product)
            .ThenInclude(p => p.UnitOfMeasure)
            .Where(i => i.Date.Day == day && i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        var report = movements
            .GroupBy(m => m.ProductID)
            .Select(g =>
            {
                var firstItem = g.First();
                return new InventoryDailyReportDTO
                {
                    ProductID = firstItem.ProductID,
                    ProductName = firstItem.Product.Name,
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity),
                    UnitOfMeasure = firstItem.Product.UnitOfMeasure.UnitName
                };
            }).ToList();

        return report;
    }


}
