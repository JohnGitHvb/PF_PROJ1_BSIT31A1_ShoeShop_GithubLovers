using System.Threading.Tasks;
using ShoeShop.Services.DTOs.PurchaseOrders;

namespace ShoeShop.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task<PurchaseOrderDto> GetPurchaseOrderAsync(int id);
        Task ReceivePurchaseOrderItemAsync(int purchaseOrderItemId, int quantityReceived);
        Task ConfirmPurchaseOrderAsync(int purchaseOrderId);
    }
}
