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

public class SvInventoryService : ISvInventoryService
{
    private readonly ISvGenericRepository<Inventory> _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IValidator<InventoryCreateDTO> _inventoryCreateValidator;
    public SvInventoryService(ISvGenericRepository<Inventory> inventoryRepository, IMapper mapper, AppDbContext context, IValidator<InventoryCreateDTO> inventoryCreateValidator)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _context = context;
        _inventoryCreateValidator = inventoryCreateValidator;
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
