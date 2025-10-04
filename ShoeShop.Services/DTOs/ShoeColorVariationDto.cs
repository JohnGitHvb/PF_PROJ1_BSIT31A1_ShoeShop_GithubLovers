namespace ShoeShop.Services.DTOs
{
    public class ShoeColorVariationDto
    {
        public int Id { get; set; }
        public int ShoeId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string? HexCode { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public bool IsLowStock => StockQuantity <= ReorderLevel;
    }
}