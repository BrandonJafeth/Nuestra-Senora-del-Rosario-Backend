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
        Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAllProductsAsync(
           int month,
           int year,
           Dictionary<int, string> conversionMapping);
        Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportAsync(
          int month,
          int year,
          string targetUnit,
          List<int> convertProductIds,
          int categoryId);

        Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryAsync(
            int month,
            int year,
            Dictionary<int, string> conversionMapping,
            int categoryId
        );

        Task<IEnumerable<InventoryReportDTO>> GetMonthlyReportByCategoryWithMovementsAsync(
            int month,
            int year,
            Dictionary<int, string> conversionMapping,
            int categoryId
        );
        Task RegisterMovementsAsync(IEnumerable<InventoryCreateDTO> inventoryCreateDTOs);

        Task PatchInventoryAsync(int inventoryId, JsonPatchDocument<Domain.Entities.Administration.Inventory> patchDoc);

        Task DeleteInventoryAsync(int inventoryId); // Método de eliminación

        Task<IEnumerable<InventoryDailyReportDTO>> GetDailyMovementsAsync(DateTime date);

        Task<IEnumerable<InventoryGetDTO>> GetMovementsByDayAsync(int day, int month, int year); // Movimientos diarios
        Task<IEnumerable<InventoryDailyReportDTO>> GetDailyReportAsync(int day, int month, int year); // Resumen de ingresos/egresos por día
    }

}
