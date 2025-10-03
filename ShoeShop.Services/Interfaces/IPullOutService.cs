using System.Collections.Generic;
using System.Threading.Tasks;
using ShoeShop.Services.DTOs.PullOuts;

namespace ShoeShop.Services.Interfaces
{
    public interface IPullOutService
    {
        Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto dto);
        Task<IEnumerable<PullOutRequestDto>> GetPendingPullOutsAsync();
        Task<PullOutRequestDto> ApprovePullOutAsync(int pullOutId, string approverRole, string approverName);
        Task<PullOutRequestDto> CompletePullOutAsync(int pullOutId);
        Task RejectPullOutAsync(int pullOutId, string approverName, string reason);
    }
}
