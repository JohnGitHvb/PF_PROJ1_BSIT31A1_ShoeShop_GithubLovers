using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Shoes
{
    public class DeleteModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Shoe Shoe { get; set; }

        public DeleteModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Shoe = await _context.Shoes.FindAsync(id);
            if (Shoe == null)
            {
                return RedirectToPage("/Admin/Shoes/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var shoe = await _context.Shoes.FindAsync(Shoe.Id);
            if (shoe != null)
            {
                _context.Shoes.Remove(shoe);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Admin/Shoes/Index");
        }
    }
}
