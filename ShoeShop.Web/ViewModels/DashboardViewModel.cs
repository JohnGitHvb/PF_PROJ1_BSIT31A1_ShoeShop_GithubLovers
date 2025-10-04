using ShoeShop.Services.DTOs;

namespace ShoeShop.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalShoes { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int LowStockCount { get; set; }
        public int PendingOrdersCount { get; set; }
        public int PendingPullOutsCount { get; set; }

        public List<ShoeColorVariationDto> LowStockItems { get; set; } = new List<ShoeColorVariationDto>();
        public List<PurchaseOrderDto> RecentPurchaseOrders { get; set; } = new List<PurchaseOrderDto>();
        public List<StockPullOutDto> RecentPullOuts { get; set; } = new List<StockPullOutDto>();
    }
}