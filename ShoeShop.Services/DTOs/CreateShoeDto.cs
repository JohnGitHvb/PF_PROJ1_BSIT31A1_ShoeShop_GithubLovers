using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreateShoeDto
    {
        [Required(ErrorMessage = "Shoe name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost is required")]
        [Range(0.01, 10000, ErrorMessage = "Cost must be between $0.01 and $10,000")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }
    }
}