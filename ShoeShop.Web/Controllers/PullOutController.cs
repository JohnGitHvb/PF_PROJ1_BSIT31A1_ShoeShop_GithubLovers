using Microsoft.AspNetCore.Mvc;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Web.Controllers
{
    public class PullOutController : Controller
    {
        private readonly IStockPullOutService _pullOutService;
        private readonly IInventoryService _inventoryService;

        public PullOutController(
            IStockPullOutService pullOutService,
            IInventoryService inventoryService)
        {
            _pullOutService = pullOutService;
            _inventoryService = inventoryService;
        }

        // GET: PullOut
        public async Task<IActionResult> Index(string status)
        {
            List<StockPullOutDto> pullOuts;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<PullOutStatus>(status, out var pullOutStatus))
            {
                pullOuts = await _pullOutService.GetPullOutsByStatusAsync(pullOutStatus);
            }
            else
            {
                pullOuts = await _pullOutService.GetAllPullOutsAsync();
            }

            ViewBag.CurrentStatus = status;
            return View(pullOuts);
        }

        // GET: PullOut/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pullOut = await _pullOutService.GetPullOutByIdAsync(id);
            if (pullOut == null)
            {
                return NotFound();
            }
            return View(pullOut);
        }

        // GET: PullOut/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();
            return View();
        }

        // POST: PullOut/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStockPullOutDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();
                return View(dto);
            }

            try
            {
                await _pullOutService.CreatePullOutAsync(dto);
                TempData["SuccessMessage"] = "Pull-out request created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.ColorVariations = await _inventoryService.GetAllColorVariationsAsync();
                return View(dto);
            }
        }

        // POST: PullOut/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string approvedBy)
        {
            try
            {
                await _pullOutService.ApprovePullOutAsync(id, approvedBy);
                TempData["SuccessMessage"] = "Pull-out request approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PullOut/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                await _pullOutService.RejectPullOutAsync(id, reason);
                TempData["SuccessMessage"] = "Pull-out request rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: PullOut/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                await _pullOutService.CompletePullOutAsync(id);
                TempData["SuccessMessage"] = "Pull-out completed! Stock has been deducted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}