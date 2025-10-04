using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeShop.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShoeShop.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ShoeShopDbContext _context;
        public int TotalShoes { get; set; }
        public int LowStock { get; set; }
        public int PendingOrders { get; set; }

        public DashboardModel(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            TotalShoes = await _context.Shoes.CountAsync();
            LowStock = await _context.ShoeColorVariations.CountAsync(cv => cv.StockQuantity <= cv.ReorderLevel);
            PendingOrders = await _context.PurchaseOrders.CountAsync(po => po.Status == Entities.PurchaseOrderStatus.Pending);
        }
    }
}
