using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;

namespace ShoeShop.Services.Interfaces
{
    public interface IStockPullOutService
    {
        // Pull-Out Management
        Task<List<StockPullOutDto>> GetAllPullOutsAsync();
        Task<List<StockPullOutDto>> GetPullOutsByStatusAsync(PullOutStatus status);
        Task<StockPullOutDto?> GetPullOutByIdAsync(int id);
        Task<StockPullOutDto> CreatePullOutAsync(CreateStockPullOutDto dto);

        // Approval Workflow
        Task<bool> ApprovePullOutAsync(int pullOutId, string approvedBy);
        Task<bool> RejectPullOutAsync(int pullOutId, string reason);
        Task<bool> CompletePullOutAsync(int pullOutId);

        // Statistics
        Task<int> GetPendingPullOutsCountAsync();
        Task<List<StockPullOutDto>> GetPullOutsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}