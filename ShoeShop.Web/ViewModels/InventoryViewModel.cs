using ShoeShop.Services.DTOs;

namespace ShoeShop.Web.ViewModels
{
    public class InventoryViewModel
    {
        public List<ShoeDto> Shoes { get; set; } = new List<ShoeDto>();
        public List<ShoeColorVariationDto> ColorVariations { get; set; } = new List<ShoeColorVariationDto>();
        public string? SearchTerm { get; set; }
        public string? FilterBrand { get; set; }
        public bool ShowLowStockOnly { get; set; }
    }
}