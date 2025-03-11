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
    List<int> convertProductIds)
    {
        // Obtenemos los movimientos de inventario correspondientes al mes indicado
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // Extraemos los IDs de producto que tuvieron movimiento en el mes
        var productIds = monthlyMovements.Select(m => m.ProductID).Distinct().ToList();

        // Obtenemos solo los productos que tuvieron movimiento
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.ProductID))
            .ToListAsync();

        // Agrupamos los movimientos por producto para calcular ingresos y egresos del mes
        var monthlyData = monthlyMovements.GroupBy(m => m.ProductID)
            .ToDictionary(g => g.Key, g => new {
                TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
            });

        var report = products.Select(p =>
        {
            // Total global del producto actualizado (almacenado en Product.TotalQuantity)
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;
            string unitToShow = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Obtener los movimientos mensuales (si existen) para el producto
            int totalIngress = 0, totalEgress = 0;
            if (monthlyData.ContainsKey(p.ProductID))
            {
                totalIngress = monthlyData[p.ProductID].TotalIngresos;
                totalEgress = monthlyData[p.ProductID].TotalEgresos;
            }

            // Aplica la conversión solo para los productos que estén en la lista de conversión
            if (!string.IsNullOrEmpty(targetUnit) &&
                convertProductIds != null &&
                convertProductIds.Contains(p.ProductID))
            {
                switch (targetUnit.ToLower())
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        unitToShow = "Paquete(s)";
                        break;
                    case "kg":
                        // Ejemplo: para productos en gramos, 1 kg = 1000 gramos
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        unitToShow = "kg";
                        break;
                    case "caja":
                        // Si el producto se mide en litros (ej. leche), usamos la conversión a caja.
                        if (p.UnitOfMeasure?.UnitName.ToLower().Contains("litro") == true)
                        {
                            // ConvertMilk asume que 1 caja equivale a 12 litros
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            unitToShow = "Caja(s)";
                        }
                        break;
                    default:
                        // Sin conversión, se mantiene la unidad base
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                // Total global actualizado (en la unidad base)
                TotalInStock = totalQuantity,
                // Movimientos mensuales calculados a partir de la tabla Inventory
                TotalIngresos = totalIngress,
                TotalEgresos = totalEgress,
                UnitOfMeasure = unitToShow,
                ConvertedTotalInStock = convertedQuantity
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
     string targetUnit,
     List<int> convertProductIds)
    {
        // Obtenemos todos los productos (sin filtrar, se mostrarán aunque no hayan tenido movimientos)
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .ToListAsync();

        // Obtenemos los movimientos de inventario correspondientes al mes indicado
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // Agrupamos los movimientos por producto para calcular ingresos y egresos del mes
        var monthlyData = monthlyMovements.GroupBy(m => m.ProductID)
            .ToDictionary(g => g.Key, g => new {
                TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
            });

        var report = products.Select(p =>
        {
            // Stock global del producto (actualizado por trigger)
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;
            string unitToShow = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Si existen movimientos del mes para este producto, se extraen ingresos y egresos;
            // en caso contrario, quedan en 0.
            int totalIngress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalIngresos : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalEgresos : 0;

            // Aplica la conversión si se indicó un targetUnit y el producto está en la lista de conversión.
            if (!string.IsNullOrEmpty(targetUnit) &&
                convertProductIds != null &&
                convertProductIds.Contains(p.ProductID))
            {
                switch (targetUnit.ToLower())
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        unitToShow = "Paquete(s)";
                        break;
                    case "kg":
                        // Ejemplo: para productos en gramos, 1 kg = 1000 gramos
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        unitToShow = "kg";
                        break;
                    case "caja":
                        // Si el producto se mide en litros (por ejemplo, leche), usamos la conversión a caja.
                        if (p.UnitOfMeasure?.UnitName.ToLower().Contains("litro") == true)
                        {
                            // ConvertMilk asume que 1 caja equivale a 12 litros
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            unitToShow = "Caja(s)";
                        }
                        break;
                    default:
                        // Sin conversión, se mantiene la unidad base.
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,       // Stock global (sin conversión)
                TotalIngresos = totalIngress,         // Ingresos del mes (0 si no hubo movimientos)
                TotalEgresos = totalEgress,           // Egresos del mes (0 si no hubo movimientos)
                UnitOfMeasure = unitToShow,
                ConvertedTotalInStock = convertedQuantity
            };
        }).ToList();

        return report;
    }

    // Nuevo método: Reporte mensual filtrado por categoría
    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryAsync(
        int month,
        int year,
        string targetUnit,
        List<int> convertProductIds,
        int categoryId)
    {
        // Obtenemos los productos que pertenecen a la categoría obligatoria
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .Where(p => p.CategoryID == categoryId)
            .ToListAsync();

        // Obtenemos los movimientos de inventario correspondientes al mes indicado
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // Agrupamos los movimientos por producto para calcular ingresos y egresos del mes
        var monthlyData = monthlyMovements.GroupBy(m => m.ProductID)
            .ToDictionary(g => g.Key, g => new {
                TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
            });

        var report = products.Select(p =>
        {
            // Stock global del producto actualizado (almacenado en Product.TotalQuantity)
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;
            string unitToShow = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Si existen movimientos mensuales para este producto, extraemos ingresos y egresos
            int totalIngress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalIngresos : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalEgresos : 0;

            // Aplica la conversión solo para los productos que estén en la lista de conversión
            if (!string.IsNullOrEmpty(targetUnit) &&
                convertProductIds != null &&
                convertProductIds.Contains(p.ProductID))
            {
                switch (targetUnit.ToLower())
                {
                    case "paquete":
                        // Ejemplo: 1 paquete = 20 unidades
                        convertedQuantity = totalQuantity / 20.0;
                        unitToShow = "Paquete(s)";
                        break;
                    case "kg":
                        // Ejemplo: para productos en gramos, 1 kg = 1000 gramos
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        unitToShow = "kg";
                        break;
                    case "caja":
                        // Si el producto se mide en litros (ej. leche), usamos la conversión a caja.
                        if (p.UnitOfMeasure?.UnitName.ToLower().Contains("litro") == true)
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            unitToShow = "Caja(s)";
                        }
                        break;
                    default:
                        // Sin conversión, se mantiene la unidad base
                        break;
                }
            }

            return new InventoryReportDTO
            {
                ProductID = p.ProductID,
                ProductName = p.Name,
                TotalInStock = totalQuantity,       // Stock global (unidad base)
                TotalIngresos = totalIngress,         // Ingresos del mes (0 si no hubo movimiento)
                TotalEgresos = totalEgress,           // Egresos del mes (0 si no hubo movimiento)
                UnitOfMeasure = unitToShow,
                ConvertedTotalInStock = convertedQuantity
            };
        }).ToList();

        return report;
    }


    // Nuevo método: Reporte mensual filtrado por categoría y que solo muestre productos con movimiento.
    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryWithMovementsAsync(
        int month,
        int year,
        string targetUnit,
        List<int> convertProductIds,
        int categoryId)
    {
        // 1. Obtener los movimientos del mes
        var monthlyMovements = await _inventoryRepository.Query()
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        // 2. Extraer los IDs de producto que tuvieron movimiento
        var productIds = monthlyMovements.Select(m => m.ProductID).Distinct().ToList();

        // 3. Filtrar productos que pertenezcan a la categoría indicada y hayan tenido movimiento
        var products = await _context.Set<Product>()
            .Include(p => p.UnitOfMeasure)
            .Include(p => p.Category)
            .Where(p => p.CategoryID == categoryId && productIds.Contains(p.ProductID))
            .ToListAsync();

        // 4. Agrupar movimientos por producto para calcular ingresos y egresos
        var monthlyData = monthlyMovements.GroupBy(m => m.ProductID)
            .ToDictionary(g => g.Key, g => new
            {
                TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity)
            });

        // 5. Construir el reporte para cada producto
        var report = products.Select(p =>
        {
            int totalQuantity = p.TotalQuantity;
            double convertedQuantity = totalQuantity;
            string unitToShow = p.UnitOfMeasure?.UnitName ?? "Unknown";

            // Si existen movimientos mensuales, extraer ingresos y egresos
            int totalIngress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalIngresos : 0;
            int totalEgress = monthlyData.ContainsKey(p.ProductID) ? monthlyData[p.ProductID].TotalEgresos : 0;

            // Aplicar conversión para los productos indicados
            if (!string.IsNullOrEmpty(targetUnit) &&
                convertProductIds != null &&
                convertProductIds.Contains(p.ProductID))
            {
                switch (targetUnit.ToLower())
                {
                    case "paquete":
                        convertedQuantity = totalQuantity / 20.0;
                        unitToShow = "Paquete(s)";
                        break;
                    case "kg":
                        convertedQuantity = _conversionService.ConvertGramsToKilograms(totalQuantity);
                        unitToShow = "kg";
                        break;
                    case "caja":
                        if (p.UnitOfMeasure?.UnitName.ToLower().Contains("litro") == true)
                        {
                            var milkConversion = _conversionService.ConvertMilk(totalQuantity, 12);
                            convertedQuantity = milkConversion.Boxes;
                            unitToShow = "Caja(s)";
                        }
                        break;
                    default:
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
                UnitOfMeasure = unitToShow,
                ConvertedTotalInStock = convertedQuantity
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
