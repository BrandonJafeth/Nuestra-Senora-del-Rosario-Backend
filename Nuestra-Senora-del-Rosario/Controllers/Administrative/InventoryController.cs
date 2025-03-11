using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.Inventory;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ISvInventoryService _inventoryService;

    public InventoryController(ISvInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET: api/inventory
    [HttpGet]
    public async Task<IActionResult> GetAllMovements()
    {
        var movements = await _inventoryService.GetAllMovementsAsync();
        return Ok(movements);
    }

    // GET: api/inventory/movements/month
    [HttpGet("movements/month")]
    public async Task<IActionResult> GetMovementsByMonth([FromQuery] int month, [FromQuery] int year)
    {
        var movements = await _inventoryService.GetMovementsByMonthAsync(month, year);
        return Ok(movements);
    }

    // GET: api/inventory/report/month
    // GET: api/inventory/report/month
    // Se puede enviar en la query: targetUnit (por ejemplo "paquete", "kg") y productIds (ej. "1,3,5")
    [HttpGet("report/all/month")]
    public async Task<IActionResult> GetMonthlyReportAllProductsCombo(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] string productIds,
        [FromQuery] string targetUnits)
    {
        // Parsear productIds y targetUnits a listas
        List<int> productIdList = null;
        List<string> targetUnitList = null;

        if (!string.IsNullOrEmpty(productIds))
        {
            productIdList = productIds
                .Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();
        }

        if (!string.IsNullOrEmpty(targetUnits))
        {
            targetUnitList = targetUnits
                .Split(',')
                .Select(u => u.Trim())
                .ToList();
        }

        if (productIdList == null || targetUnitList == null || productIdList.Count != targetUnitList.Count)
        {
            return BadRequest("Las listas de productIds y targetUnits deben tener la misma longitud y no estar vacías.");
        }

        // Combinar en un diccionario: productId -> targetUnit
        var conversionMapping = new Dictionary<int, string>();
        for (int i = 0; i < productIdList.Count; i++)
        {
            conversionMapping[productIdList[i]] = targetUnitList[i];
        }

        var report = await _inventoryService.GetMonthlyReportAllProductsAsync(month, year, conversionMapping);
        return Ok(report);
    }

    // GET: api/inventory/report/{productId}/month
    [HttpGet("report/month")]
    public async Task<IActionResult> GetMonthlyReport(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] string targetUnit = null,
        [FromQuery] string productIds = null)
    {
        List<int> convertProductIds = null;
        if (!string.IsNullOrEmpty(productIds))
        {
            convertProductIds = productIds.Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();
        }

        var report = await _inventoryService.GetMonthlyReportAsync(month, year, targetUnit, convertProductIds);
        return Ok(report);
    }


    [HttpGet("report/category/month")]
    public async Task<IActionResult> GetMonthlyReportByCategoryCombo(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] int categoryId,
        [FromQuery] string productIds,
        [FromQuery] string targetUnits)
    {
        // 1. Parsear las listas
        List<int> productIdList = null;
        List<string> targetUnitList = null;

        if (!string.IsNullOrEmpty(productIds))
        {
            productIdList = productIds
                .Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();
        }

        if (!string.IsNullOrEmpty(targetUnits))
        {
            targetUnitList = targetUnits
                .Split(',')
                .Select(u => u.Trim())
                .ToList();
        }

        // 2. Validar que tengan la misma longitud
        if (productIdList == null || targetUnitList == null || productIdList.Count != targetUnitList.Count)
        {
            return BadRequest("productIds y targetUnits deben tener la misma longitud y no estar vacíos.");
        }

        // 3. Crear el diccionario: productId -> targetUnit
        var conversionMapping = new Dictionary<int, string>();
        for (int i = 0; i < productIdList.Count; i++)
        {
            conversionMapping[productIdList[i]] = targetUnitList[i];
        }

        // 4. Llamar al servicio
        var report = await _inventoryService.GetMonthlyReportByCategoryAsync(
            month,
            year,
            conversionMapping,
            categoryId
        );

        return Ok(report);
    }

    // GET: api/inventory/report/category/movements?month=3&year=2025&categoryId=2&targetUnit=paquete&productIds=1,4,7
    [HttpGet("report/category/movements")]
    public async Task<IActionResult> GetMonthlyReportByCategoryWithMovementsCombo(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] int categoryId,
        [FromQuery] string productIds,
        [FromQuery] string targetUnits)
    {
        // 1. Parsear las listas separadas por comas
        List<int> productIdList = null;
        List<string> targetUnitList = null;

        if (!string.IsNullOrEmpty(productIds))
        {
            productIdList = productIds
                .Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();
        }

        if (!string.IsNullOrEmpty(targetUnits))
        {
            targetUnitList = targetUnits
                .Split(',')
                .Select(u => u.Trim())
                .ToList();
        }

        // Validar que ambas listas existan y tengan la misma cantidad de elementos
        if (productIdList == null || targetUnitList == null
            || productIdList.Count != targetUnitList.Count)
        {
            return BadRequest("Las listas de productIds y targetUnits deben tener la misma longitud y no estar vacías.");
        }

        // 2. Combinar en un diccionario: productId -> targetUnit
        var conversionMapping = new Dictionary<int, string>();
        for (int i = 0; i < productIdList.Count; i++)
        {
            conversionMapping[productIdList[i]] = targetUnitList[i];
        }

        // 3. Llamar al método del servicio, pasando el mapping y el categoryId
        var report = await _inventoryService.GetMonthlyReportByCategoryWithMovementsAsync(
            month,
            year,
            conversionMapping,
            categoryId
        );

        return Ok(report);
    }

    // POST: api/inventory
    [HttpPost]
    public async Task<IActionResult> RegisterMovements([FromBody] List<InventoryCreateDTO> dtos)
    {
        if (dtos == null || dtos.Count == 0)
        {
            return BadRequest("La lista de movimientos está vacía.");
        }

        try
        {
            await _inventoryService.RegisterMovementsAsync(dtos);
            return Ok("Movimientos registrados exitosamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al registrar los movimientos: {ex.Message}");
        }
    }

    // PATCH: api/inventory/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchInventory(int id, [FromBody] JsonPatchDocument<Inventory> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        await _inventoryService.PatchInventoryAsync(id, patchDoc);
        return NoContent();
    }

    // DELETE: api/inventory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        await _inventoryService.DeleteInventoryAsync(id);
        return NoContent();
    }

    [HttpGet("movements/daily")]
    public async Task<IActionResult> GetDailyMovements([FromQuery] DateTime date)
    {
        var dailyReport = await _inventoryService.GetDailyMovementsAsync(date);
        return Ok(dailyReport);
    }


    // GET: api/inventory/movements/day
    [HttpGet("movements/day")]
    public async Task<IActionResult> GetMovementsByDay([FromQuery] int day, [FromQuery] int month, [FromQuery] int year)
    {
        var movements = await _inventoryService.GetMovementsByDayAsync(day, month, year);
        return Ok(movements);
    }

    // GET: api/inventory/report/day
    [HttpGet("report/day")]
    public async Task<IActionResult> GetDailyReport([FromQuery] int day, [FromQuery] int month, [FromQuery] int year)
    {
        var report = await _inventoryService.GetDailyReportAsync(day, month, year);
        return Ok(report);
    }

}
