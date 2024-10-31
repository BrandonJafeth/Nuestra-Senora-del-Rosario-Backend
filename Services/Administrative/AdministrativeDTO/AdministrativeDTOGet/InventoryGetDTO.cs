namespace Services.Administrative.AdministrativeDTO.AdministrativeDTOGet
{
    public class InventoryGetDTO
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // Nombre del producto
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string MovementType { get; set; }
    }
}
