using System.Threading.Tasks;
using ShoeShop.Services.DTOs.Reports;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IInventoryService _inventory;
        public ReportService(IInventoryService inventory) => _inventory = inventory;

        public Task<InventoryReportDto> GenerateInventoryReportAsync()
        {
            return _inventory.GetInventoryReportAsync();
        }
    }
}
