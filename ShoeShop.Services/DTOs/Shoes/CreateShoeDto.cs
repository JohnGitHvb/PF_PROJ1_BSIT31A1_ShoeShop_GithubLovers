using System;
using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.Shoes
{
    public class CreateShoeDto
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required, StringLength(100)]
        public string Brand { get; set; }

        [Required, Range(0.01, 1000000)]
        public decimal Cost { get; set; }

        [Required, Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
