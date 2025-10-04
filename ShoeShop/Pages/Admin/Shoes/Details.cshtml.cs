using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Shoes
{
    public class DetailsModel : PageModel
    {
        private readonly ShoeShopDbContext _context;
        public Shoe Shoe { get; set; }

        public DetailsModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Shoe = await _context.Shoes
                .Include(s => s.ColorVariations)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Shoe == null)
            {
                return RedirectToPage("/Admin/Shoes/Index");
            }
            return Page();
        }
    }
}
