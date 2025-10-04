using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreateStockPullOutDto
    {
        [Required(ErrorMessage = "Shoe color variation is required")]
        public int ShoeColorVariationId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10,000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(100, ErrorMessage = "Reason cannot exceed 100 characters")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Details cannot exceed 500 characters")]
        public string? ReasonDetails { get; set; }

        [Required(ErrorMessage = "Requester name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string RequestedBy { get; set; } = string.Empty;
    }
}