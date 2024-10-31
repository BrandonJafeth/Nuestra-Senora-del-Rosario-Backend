namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class InventoryReportDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int TotalInStock { get; set; } // Cantidad total en inventario
        public int TotalIngresos { get; set; } // Suma de ingresos
        public int TotalEgresos { get; set; }  // Suma de egresos
        public string UnitOfMeasure { get; set; }
    }
}
