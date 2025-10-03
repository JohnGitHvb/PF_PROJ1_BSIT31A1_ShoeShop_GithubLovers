using System.Threading.Tasks;
using ShoeShop.Services.DTOs.Reports;

namespace ShoeShop.Services.Interfaces
{
    public interface IReportService
    {
        Task<InventoryReportDto> GenerateInventoryReportAsync();
    }
}
