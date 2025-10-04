using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;

namespace ShoeShop.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IStockPullOutService _pullOutService;

        public DashboardController(
            IInventoryService inventoryService,
            IPurchaseOrderService purchaseOrderService,
            IStockPullOutService pullOutService)
        {
            _inventoryService = inventoryService;
            _purchaseOrderService = purchaseOrderService;
            _pullOutService = pullOutService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalShoes = (await _inventoryService.GetAllShoesAsync()).Count,
                TotalStock = await _inventoryService.GetTotalStockCountAsync(),
                TotalInventoryValue = await _inventoryService.GetTotalInventoryValueAsync(),
                LowStockCount = (await _inventoryService.GetLowStockItemsAsync()).Count,
                PendingOrdersCount = await _purchaseOrderService.GetPendingOrdersCountAsync(),
                PendingPullOutsCount = await _pullOutService.GetPendingPullOutsCountAsync(),
                LowStockItems = await _inventoryService.GetLowStockItemsAsync(),
                RecentPurchaseOrders = (await _purchaseOrderService.GetAllPurchaseOrdersAsync()).Take(5).ToList(),
                RecentPullOuts = (await _pullOutService.GetAllPullOutsAsync()).Take(5).ToList()
            };

            return View(viewModel);
        }
    }
}