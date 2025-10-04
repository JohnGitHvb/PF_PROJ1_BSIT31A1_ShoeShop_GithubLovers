using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin.Shoes
{
    public class CreateModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Shoe Shoe { get; set; } = new();

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
            Shoe.CreatedDate = DateTime.Now;
            _context.Shoes.Add(Shoe);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Shoes/Index");
        }
    }
}
