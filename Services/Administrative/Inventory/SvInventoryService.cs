using AutoMapper;
using Entities.Administration;
using Services.GenericService;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.Inventory;

public class SvInventoryService : ISvInventoryService
{
    private readonly ISvGenericRepository<Inventory> _inventoryRepository;
    private readonly IMapper _mapper;

    public SvInventoryService(ISvGenericRepository<Inventory> inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
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

    public async Task<InventoryReportDTO> GetMonthlyReportAsync(int productId, int month, int year)
    {
        var movements = await _inventoryRepository
            .Query()
            .Include(i => i.Product)                    // Incluye Product
            .ThenInclude(p => p.UnitOfMeasure)          // Incluye UnitOfMeasure
            .Include(i => i.Product.Category)           // Incluye Category
            .Where(i => i.ProductID == productId && i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        var totalIngress = movements.Where(i => i.MovementType == "Ingreso").Sum(i => i.Quantity);
        var totalEgress = movements.Where(i => i.MovementType == "Egreso").Sum(i => i.Quantity);

        return new InventoryReportDTO
        {
            ProductID = productId,
            TotalIngresos = totalIngress,
            TotalEgresos = totalEgress,
            TotalInStock = totalIngress - totalEgress,
            ProductName = movements.FirstOrDefault()?.Product?.Name ?? "Unknown",
            UnitOfMeasure = movements.FirstOrDefault()?.Product?.UnitOfMeasure?.UnitName ?? "Unknown"
        };
    }

    public async Task RegisterMovementAsync(InventoryCreateDTO inventoryCreateDTO)
    {
        var inventory = _mapper.Map<Inventory>(inventoryCreateDTO);
        await _inventoryRepository.AddAsync(inventory);
        await _inventoryRepository.SaveChangesAsync();
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


    public async Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAllProductsAsync(int month, int year)
    {
        var movements = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .ThenInclude(p => p.UnitOfMeasure)
            .Include(i => i.Product.Category)
            .Where(i => i.Date.Month == month && i.Date.Year == year)
            .ToListAsync();

        var report = movements
            .GroupBy(m => m.ProductID)
            .Select(g =>
            {
                var firstItem = g.First();
                return new InventoryReportDTO
                {
                    ProductID = firstItem.ProductID,
                    ProductName = firstItem.Product.Name,
                    TotalIngresos = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity),
                    TotalEgresos = g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity),
                    TotalInStock = g.Where(x => x.MovementType == "Ingreso").Sum(x => x.Quantity) - g.Where(x => x.MovementType == "Egreso").Sum(x => x.Quantity),
                    UnitOfMeasure = firstItem.Product.UnitOfMeasure.UnitName
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
