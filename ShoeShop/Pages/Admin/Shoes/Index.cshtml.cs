using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using ShoeShop.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShoeShop.Pages.Admin.Shoes
{
    public class IndexModel : PageModel
    {
        private readonly ShoeShopDbContext _context;
        public List<Shoe> Shoes { get; set; } = new();

        public IndexModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Shoes = await _context.Shoes.ToListAsync();
        }
    }
}
