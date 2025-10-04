using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;
using ShoeShop.Web.ViewModels;

namespace ShoeShop.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: Inventory
        public async Task<IActionResult> Index(string searchTerm, string filterBrand, bool showLowStockOnly = false)
        {
            var shoes = await _inventoryService.GetAllShoesAsync();
            var colorVariations = await _inventoryService.GetAllColorVariationsAsync();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                shoes = shoes.Where(s => s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                        s.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filterBrand))
            {
                shoes = shoes.Where(s => s.Brand == filterBrand).ToList();
                colorVariations = colorVariations.Where(cv => cv.Brand == filterBrand).ToList();
            }

            if (showLowStockOnly)
            {
                colorVariations = colorVariations.Where(cv => cv.IsLowStock).ToList();
            }

            var viewModel = new InventoryViewModel
            {
                Shoes = shoes,
                ColorVariations = colorVariations,
                SearchTerm = searchTerm,
                FilterBrand = filterBrand,
                ShowLowStockOnly = showLowStockOnly
            };

            return View(viewModel);
        }

        // GET: Inventory/CreateShoe
        public IActionResult CreateShoe()
        {
            return View();
        }

        // POST: Inventory/CreateShoe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShoe(CreateShoeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _inventoryService.CreateShoeAsync(dto);
                TempData["SuccessMessage"] = "Shoe created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        // GET: Inventory/EditShoe/5
        public async Task<IActionResult> EditShoe(int id)
        {
            var shoe = await _inventoryService.GetShoeByIdAsync(id);
            if (shoe == null)
            {
                return NotFound();
            }

            var dto = new CreateShoeDto
            {
                Name = shoe.Name,
                Brand = shoe.Brand,
                Cost = shoe.Cost,
                Price = shoe.Price,
                Description = shoe.Description,
                ImageUrl = shoe.ImageUrl
            };

            ViewData["ShoeId"] = id;
            return View(dto);
        }

        // POST: Inventory/EditShoe/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShoe(int id, CreateShoeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ShoeId"] = id;
                return View(dto);
            }

            try
            {
                await _inventoryService.UpdateShoeAsync(id, dto);
                TempData["SuccessMessage"] = "Shoe updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewData["ShoeId"] = id;
                return View(dto);
            }
        }

        // GET: Inventory/DeleteShoe/5
        public async Task<IActionResult> DeleteShoe(int id)
        {
            var shoe = await _inventoryService.GetShoeByIdAsync(id);
            if (shoe == null)
            {
                return NotFound();
            }

            return View(shoe);
        }

        // POST: Inventory/DeleteShoe/5
        [HttpPost, ActionName("DeleteShoe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShoeConfirmed(int id)
        {
            try
            {
                await _inventoryService.DeleteShoeAsync(id);
                TempData["SuccessMessage"] = "Shoe deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inventory/CreateColor
        public async Task<IActionResult> CreateColor()
        {
            ViewBag.Shoes = await _inventoryService.GetAllShoesAsync();
            return View();
        }

        // POST: Inventory/CreateColor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateColor(CreateShoeColorVariationDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Shoes = await _inventoryService.GetAllShoesAsync();
                return View(dto);
            }

            try
            {
                await _inventoryService.CreateColorVariationAsync(dto);
                TempData["SuccessMessage"] = "Color variation created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Shoes = await _inventoryService.GetAllShoesAsync();
                return View(dto);
            }
        }
    }
}