using Entities.Administration;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.Inventory;
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
    [HttpGet("report/month")]
    public async Task<IActionResult> GetMonthlyReportAllProducts([FromQuery] int month, [FromQuery] int year)
    {
        var report = await _inventoryService.GetMonthlyReportAllProductsAsync(month, year);
        return Ok(report);
    }

    // GET: api/inventory/report/{productId}/month
    [HttpGet("report/{productId}/month")]
    public async Task<IActionResult> GetMonthlyReportForProduct(int productId, [FromQuery] int month, [FromQuery] int year)
    {
        var report = await _inventoryService.GetMonthlyReportAsync(productId, month, year);
        return Ok(report);
    }

    // POST: api/inventory
    [HttpPost]
    public async Task<IActionResult> RegisterMovement([FromBody] InventoryCreateDTO inventoryCreateDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _inventoryService.RegisterMovementAsync(inventoryCreateDTO);
        return StatusCode(201); // 201 Created
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
