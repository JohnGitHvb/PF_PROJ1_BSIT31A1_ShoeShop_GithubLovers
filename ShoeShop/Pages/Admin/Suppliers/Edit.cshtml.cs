using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Suppliers
{
    public class EditModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Supplier Supplier { get; set; }

        public EditModel(ShoeShopDbContext context)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var supplierInDb = await _context.Suppliers.FindAsync(Supplier.Id);
            if (supplierInDb == null)
            {
                return RedirectToPage("/Admin/Suppliers/Index");
            }
            supplierInDb.Name = Supplier.Name;
            supplierInDb.ContactEmail = Supplier.ContactEmail;
            supplierInDb.ContactPhone = Supplier.ContactPhone;
            supplierInDb.Address = Supplier.Address;
            supplierInDb.IsActive = Supplier.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Suppliers/Index");
        }
    }
}
