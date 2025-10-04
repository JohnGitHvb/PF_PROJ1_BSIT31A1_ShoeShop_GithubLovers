using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ShoeShop.Pages.Admin.PurchaseOrders
{
    public class EditModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public PurchaseOrder PurchaseOrder { get; set; }
        public SelectList SupplierOptions { get; set; }

        public EditModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PurchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (PurchaseOrder == null)
            {
                return RedirectToPage("/Admin/PurchaseOrders/Index");
            }
            SupplierOptions = new SelectList(_context.Suppliers.Where(s => s.IsActive).ToList(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SupplierOptions = new SelectList(_context.Suppliers.Where(s => s.IsActive).ToList(), "Id", "Name");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var poInDb = await _context.PurchaseOrders.FindAsync(PurchaseOrder.Id);
            if (poInDb == null)
            {
                return RedirectToPage("/Admin/PurchaseOrders/Index");
            }
            poInDb.OrderNumber = PurchaseOrder.OrderNumber;
            poInDb.SupplierId = PurchaseOrder.SupplierId;
            poInDb.OrderDate = PurchaseOrder.OrderDate;
            poInDb.ExpectedDate = PurchaseOrder.ExpectedDate;
            poInDb.Status = PurchaseOrder.Status;
            poInDb.TotalAmount = PurchaseOrder.TotalAmount;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/PurchaseOrders/Index");
        }
    }
}
