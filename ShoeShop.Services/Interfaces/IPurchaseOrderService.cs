using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;

namespace ShoeShop.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        // Purchase Order Management
        Task<List<PurchaseOrderDto>> GetAllPurchaseOrdersAsync();
        Task<List<PurchaseOrderDto>> GetPurchaseOrdersByStatusAsync(OrderStatus status);
        Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(int orderId);

        // Receiving Orders
        Task<bool> ReceiveOrderAsync(int orderId);
        Task<bool> ReceiveOrderItemAsync(int orderItemId, int quantityReceived);

        // Statistics
        Task<decimal> GetTotalPurchaseAmountAsync();
        Task<int> GetPendingOrdersCountAsync();
    }
}