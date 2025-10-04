namespace ShoeShop.Services.DTOs
{
    public class PurchaseOrderItemDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ShoeColorVariationId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost => QuantityOrdered * UnitCost;
    }
}