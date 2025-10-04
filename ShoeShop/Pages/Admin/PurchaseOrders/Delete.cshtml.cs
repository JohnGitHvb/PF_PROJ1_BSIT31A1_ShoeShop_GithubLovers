using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.PurchaseOrders
{
    public class DeleteModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public PurchaseOrder PurchaseOrder { get; set; }

        public DeleteModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PurchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .FirstOrDefaultAsync(po => po.Id == id);
            if (PurchaseOrder == null)
            {
                return RedirectToPage("/Admin/PurchaseOrders/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var po = await _context.PurchaseOrders.FindAsync(PurchaseOrder.Id);
            if (po != null)
            {
                _context.PurchaseOrders.Remove(po);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Admin/PurchaseOrders/Index");
        }
    }
}
