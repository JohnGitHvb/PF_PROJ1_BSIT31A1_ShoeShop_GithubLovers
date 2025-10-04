using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Suppliers
{
    public class DetailsModel : PageModel
    {
        private readonly ShoeShopDbContext _context;
        public Supplier Supplier { get; set; }

        public DetailsModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Supplier = await _context.Suppliers
                .Include(s => s.PurchaseOrders)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Supplier == null)
            {
                return RedirectToPage("/Admin/Suppliers/Index");
            }
            return Page();
        }
    }
}
