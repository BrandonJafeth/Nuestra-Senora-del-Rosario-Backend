using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Inventory
{
    public interface ISvInventoryService
    {
        Task<IEnumerable<InventoryGetDTO>> GetAllMovementsAsync();
        Task<IEnumerable<InventoryGetDTO>> GetMovementsByMonthAsync(int month, int year); // Nuevo método
        Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAllProductsAsync(int month, int year); // Nuevo método
        Task<InventoryReportDTO> GetMonthlyReportAsync(int productId, int month, int year);
        Task RegisterMovementAsync(InventoryCreateDTO inventoryCreateDTO);
        Task PatchInventoryAsync(int inventoryId, JsonPatchDocument<Domain.Entities.Administration.Inventory> patchDoc);

        Task DeleteInventoryAsync(int inventoryId); // Método de eliminación

        Task<IEnumerable<InventoryDailyReportDTO>> GetDailyMovementsAsync(DateTime date);

        Task<IEnumerable<InventoryGetDTO>> GetMovementsByDayAsync(int day, int month, int year); // Movimientos diarios
        Task<IEnumerable<InventoryDailyReportDTO>> GetDailyReportAsync(int day, int month, int year); // Resumen de ingresos/egresos por día
    }

}
