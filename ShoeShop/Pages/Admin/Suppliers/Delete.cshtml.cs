using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Suppliers
{
    public class DeleteModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Supplier Supplier { get; set; }

        public DeleteModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Supplier = await _context.Suppliers.FindAsync(id);
            if (Supplier == null)
            {
                return RedirectToPage("/Admin/Suppliers/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var supplier = await _context.Suppliers.FindAsync(Supplier.Id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Admin/Suppliers/Index");
        }
    }
}
