using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShoeShop.Pages.Admin.Shoes
{
    public class EditModel : PageModel
    {
        private readonly ShoeShopDbContext _context;

        [BindProperty]
        public Shoe Shoe { get; set; } = new();

        public EditModel(ShoeShopDbContext context)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var shoeInDb = await _context.Shoes.FindAsync(Shoe.Id);
            if (shoeInDb == null)
            {
                return RedirectToPage("/Admin/Shoes/Index");
            }
            shoeInDb.Name = Shoe.Name;
            shoeInDb.Brand = Shoe.Brand;
            shoeInDb.Cost = Shoe.Cost;
            shoeInDb.Price = Shoe.Price;
            shoeInDb.Description = Shoe.Description;
            shoeInDb.ImageUrl = Shoe.ImageUrl;
            shoeInDb.IsActive = Shoe.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Shoes/Index");
        }
    }
}
