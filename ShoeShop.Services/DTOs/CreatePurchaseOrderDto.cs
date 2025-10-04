using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreatePurchaseOrderDto
    {
        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Expected delivery date is required")]
        [DataType(DataType.Date)]
        public DateTime ExpectedDate { get; set; }

        [Required(ErrorMessage = "At least one item is required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CreatePurchaseOrderItemDto> OrderItems { get; set; } = new List<CreatePurchaseOrderItemDto>();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required(ErrorMessage = "Shoe color variation is required")]
        public int ShoeColorVariationId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10,000")]
        public int QuantityOrdered { get; set; }

        [Required(ErrorMessage = "Unit cost is required")]
        [Range(0.01, 10000, ErrorMessage = "Unit cost must be between $0.01 and $10,000")]
        public decimal UnitCost { get; set; }
    }
}