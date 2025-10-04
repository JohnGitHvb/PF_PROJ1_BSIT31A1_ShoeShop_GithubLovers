using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreateShoeColorVariationDto
    {
        [Required(ErrorMessage = "Shoe selection is required")]
        public int ShoeId { get; set; }

        [Required(ErrorMessage = "Color name is required")]
        [StringLength(100, ErrorMessage = "Color name cannot exceed 100 characters")]
        public string ColorName { get; set; } = string.Empty;

        [StringLength(7, MinimumLength = 7, ErrorMessage = "Hex code must be exactly 7 characters (e.g., #FF0000)")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid hex code format (e.g., #FF0000)")]
        public string? HexCode { get; set; }

        [Required(ErrorMessage = "Initial stock quantity is required")]
        [Range(0, 10000, ErrorMessage = "Stock quantity must be between 0 and 10,000")]
        public int StockQuantity { get; set; }

        [Range(1, 1000, ErrorMessage = "Reorder level must be between 1 and 1,000")]
        public int ReorderLevel { get; set; } = 5;
    }
}