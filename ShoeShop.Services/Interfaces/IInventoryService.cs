using ShoeShop.Services.DTOs;

namespace ShoeShop.Services.Interfaces
{
    public interface IInventoryService
    {
        // Shoe Management
        Task<List<ShoeDto>> GetAllShoesAsync();
        Task<ShoeDto?> GetShoeByIdAsync(int id);
        Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto);
        Task<ShoeDto> UpdateShoeAsync(int id, CreateShoeDto dto);
        Task<bool> DeleteShoeAsync(int id);

        // Color Variation Management
        Task<List<ShoeColorVariationDto>> GetAllColorVariationsAsync();
        Task<List<ShoeColorVariationDto>> GetColorVariationsByShoeIdAsync(int shoeId);
        Task<ShoeColorVariationDto?> GetColorVariationByIdAsync(int id);
        Task<ShoeColorVariationDto> CreateColorVariationAsync(CreateShoeColorVariationDto dto);
        Task<ShoeColorVariationDto> UpdateColorVariationAsync(int id, CreateShoeColorVariationDto dto);
        Task<bool> DeleteColorVariationAsync(int id);

        // Stock Management
        Task<bool> AdjustStockAsync(int colorVariationId, int quantity, string reason);
        Task<List<ShoeColorVariationDto>> GetLowStockItemsAsync();
        Task<int> GetTotalStockCountAsync();
        Task<decimal> GetTotalInventoryValueAsync();
    }
}