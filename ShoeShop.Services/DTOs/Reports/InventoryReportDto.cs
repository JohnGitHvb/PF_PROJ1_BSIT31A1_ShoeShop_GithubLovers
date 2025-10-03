namespace ShoeShop.Services.DTOs.Reports
{
    public class InventoryReportDto
    {
        public int TotalSkuCount { get; set; }
        public int TotalUnitsInStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockCount { get; set; }
    }
}
