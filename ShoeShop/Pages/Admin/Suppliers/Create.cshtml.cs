using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Suppliers
{
    public class CreateModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Supplier Supplier { get; set; } = new();

        public CreateModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Suppliers.Add(Supplier);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Suppliers/Index");
        }
    }
}
