using ShoeShop.Repository.Entities;

namespace ShoeShop.Services.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderItemDto> OrderItems { get; set; } = new List<PurchaseOrderItemDto>();
    }
}