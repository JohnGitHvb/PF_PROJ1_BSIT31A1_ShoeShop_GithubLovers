using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;

namespace ShoeShop.Pages.Admin.PurchaseOrders
{
    public class CreateModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public PurchaseOrder PurchaseOrder { get; set; } = new();
        public SelectList SupplierOptions { get; set; }

        public CreateModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            SupplierOptions = new SelectList(_context.Suppliers.Where(s => s.IsActive).ToList(), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SupplierOptions = new SelectList(_context.Suppliers.Where(s => s.IsActive).ToList(), "Id", "Name");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            PurchaseOrder.OrderDate = PurchaseOrder.OrderDate == default ? DateTime.Now : PurchaseOrder.OrderDate;
            _context.PurchaseOrders.Add(PurchaseOrder);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/PurchaseOrders/Index");
        }
    }
}
