using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.PurchaseOrders
{
    public class CreatePurchaseOrderDto
    {
        [Required, StringLength(100)]
        public string OrderNumber { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDate { get; set; }

        [Required, MinLength(1)]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public int ShoeColorVariationId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int QuantityOrdered { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }
}
