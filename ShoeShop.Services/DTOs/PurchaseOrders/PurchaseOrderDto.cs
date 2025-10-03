using System;
using System.Collections.Generic;
using ShoeShop.Services.Enums;

namespace ShoeShop.Services.DTOs.PurchaseOrders
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class PurchaseOrderItemDto
    {
        public int Id { get; set; }
        public int ShoeColorVariationId { get; set; }
        public string ColorName { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal => UnitCost * QuantityReceived;
    }
}
