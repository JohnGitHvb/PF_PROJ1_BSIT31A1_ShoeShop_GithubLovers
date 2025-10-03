using System.Collections.Generic;
using System.Threading.Tasks;
using ShoeShop.Services.DTOs.Shoes;
using ShoeShop.Services.DTOs.Reports;

namespace ShoeShop.Services.Interfaces
{
    public class ShoeColorVariationLowStockDto
    {
        public int ShoeColorVariationId { get; set; }
        public string ShoeName { get; set; }
        public string ColorName { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }

    public interface IInventoryService
    {
        Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto);
        Task<ShoeDto> GetShoeAsync(int id);
        Task<IEnumerable<ShoeDto>> GetAllShoesAsync();
        Task AddStockAsync(int shoeColorVariationId, int quantity, string note = null);
        Task RemoveStockAsync(int shoeColorVariationId, int quantity, string note = null);
        Task<IEnumerable<ShoeColorVariationLowStockDto>> GetLowStockAsync(int threshold = 5);
        Task<InventoryReportDto> GetInventoryReportAsync();
    }
}
