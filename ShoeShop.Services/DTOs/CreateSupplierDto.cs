using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? ContactPhone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }
    }
}