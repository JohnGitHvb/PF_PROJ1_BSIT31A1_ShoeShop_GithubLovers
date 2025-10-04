using Microsoft.AspNetCore.Mvc;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryService _inventoryService;

        public PurchaseOrderController(
            IPurchaseOrderService purchaseOrderService,
            ISupplierService supplierService,
            IInventoryService inventoryService)
        {
            _purchaseOrderService = purchaseOrderService;
            _supplierService = supplierService;
            _inventoryService = inventoryService;
        }

        // GET: PurchaseOrder
        public async Task<IActionResult> Index(string status)
        {
            List<PurchaseOrderDto> orders;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                orders = await _purchaseOrderService.GetPurchaseOrdersByStatusAsync(orderStatus);
            }
            else
            {
                orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
            }

            ViewBag.CurrentStatus = status;
            return View(orders);
        }

        // GET: PurchaseOrder/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET: PurchaseOrder/Create
        public async Task<IActionResult> Create(int? supplierId)
        {
            ViewBag.Suppliers = await _supplierService.GetActiveSuppliersAsync();
            ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();

            var model = new CreatePurchaseOrderDto
            {
                ExpectedDate = DateTime.Now.AddDays(7)
            };

            if (supplierId.HasValue)
            {
                model.SupplierId = supplierId.Value;
            }

            return View(model);
        }

        // POST: PurchaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = await _supplierService.GetActiveSuppliersAsync();
                ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();
                return View(dto);
            }

            try
            {
                await _purchaseOrderService.CreatePurchaseOrderAsync(dto);
                TempData["SuccessMessage"] = "Purchase order created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Suppliers = await _supplierService.GetActiveSuppliersAsync();
                ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();
                return View(dto);
            }
        }

        // POST: PurchaseOrder/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                TempData["ErrorMessage"] = "Invalid status";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _purchaseOrderService.UpdateOrderStatusAsync(id, orderStatus);
                TempData["SuccessMessage"] = $"Order status updated to {status}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PurchaseOrder/ReceiveOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveOrder(int id)
        {
            try
            {
                await _purchaseOrderService.ReceiveOrderAsync(id);
                TempData["SuccessMessage"] = "Order received successfully! Stock has been updated.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PurchaseOrder/ReceiveItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveItem(int itemId, int quantityReceived, int orderId)
        {
            try
            {
                await _purchaseOrderService.ReceiveOrderItemAsync(itemId, quantityReceived);
                TempData["SuccessMessage"] = "Item received successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // POST: PurchaseOrder/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _purchaseOrderService.CancelOrderAsync(id);
                TempData["SuccessMessage"] = "Purchase order cancelled successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}