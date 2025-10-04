using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ShoeShopDbContext _context;

        public PurchaseOrderService(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
        {
            var orders = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.ShoeColorVariation)
                        .ThenInclude(cv => cv.Shoe)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<List<PurchaseOrderDto>> GetPurchaseOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.ShoeColorVariation)
                        .ThenInclude(cv => cv.Shoe)
                .Where(po => po.Status == status)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.ShoeColorVariation)
                        .ThenInclude(cv => cv.Shoe)
                .FirstOrDefaultAsync(po => po.Id == id);

            return order == null ? null : MapToDto(order);
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            // Validate supplier exists
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
                throw new Exception($"Supplier with ID {dto.SupplierId} not found");

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Calculate total amount
            decimal totalAmount = dto.OrderItems.Sum(item => item.QuantityOrdered * item.UnitCost);

            var purchaseOrder = new PurchaseOrder
            {
                OrderNumber = orderNumber,
                SupplierId = dto.SupplierId,
                OrderDate = DateTime.Now,
                ExpectedDate = dto.ExpectedDate,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount
            };

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            // Add order items
            foreach (var itemDto in dto.OrderItems)
            {
                var orderItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = purchaseOrder.Id,
                    ShoeColorVariationId = itemDto.ShoeColorVariationId,
                    QuantityOrdered = itemDto.QuantityOrdered,
                    QuantityReceived = 0,
                    UnitCost = itemDto.UnitCost
                };

                _context.PurchaseOrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            // Reload with all includes
            return (await GetPurchaseOrderByIdAsync(purchaseOrder.Id))!;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.PurchaseOrders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.PurchaseOrders.FindAsync(orderId);
            if (order == null) return false;

            if (order.Status == OrderStatus.Received)
                throw new Exception("Cannot cancel an order that has already been received");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReceiveOrderAsync(int orderId)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.ShoeColorVariation)
                .FirstOrDefaultAsync(po => po.Id == orderId);

            if (order == null) return false;

            if (order.Status == OrderStatus.Received)
                throw new Exception("Order has already been received");

            // Update stock for all items
            foreach (var item in order.OrderItems)
            {
                item.QuantityReceived = item.QuantityOrdered;
                item.ShoeColorVariation.StockQuantity += item.QuantityOrdered;
            }

            order.Status = OrderStatus.Received;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReceiveOrderItemAsync(int orderItemId, int quantityReceived)
        {
            var orderItem = await _context.PurchaseOrderItems
                .Include(oi => oi.ShoeColorVariation)
                .Include(oi => oi.PurchaseOrder)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

            if (orderItem == null) return false;

            if (quantityReceived > orderItem.QuantityOrdered)
                throw new Exception("Received quantity cannot exceed ordered quantity");

            // Update stock
            var additionalQuantity = quantityReceived - orderItem.QuantityReceived;
            orderItem.ShoeColorVariation.StockQuantity += additionalQuantity;
            orderItem.QuantityReceived = quantityReceived;

            // Check if all items are fully received
            var allItemsReceived = await _context.PurchaseOrderItems
                .Where(oi => oi.PurchaseOrderId == orderItem.PurchaseOrderId)
                .AllAsync(oi => oi.QuantityReceived == oi.QuantityOrdered);

            if (allItemsReceived)
            {
                orderItem.PurchaseOrder.Status = OrderStatus.Received;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalPurchaseAmountAsync()
        {
            return await _context.PurchaseOrders
                .Where(po => po.Status == OrderStatus.Received)
                .SumAsync(po => po.TotalAmount);
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _context.PurchaseOrders
                .CountAsync(po => po.Status == OrderStatus.Pending);
        }

        // Helper methods
        private async Task<string> GenerateOrderNumberAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.PurchaseOrders
                .CountAsync(po => po.OrderDate.Year == year);

            return $"PO-{year}-{(count + 1):D4}";
        }

        private PurchaseOrderDto MapToDto(PurchaseOrder order)
        {
            return new PurchaseOrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier.Name,
                OrderDate = order.OrderDate,
                ExpectedDate = order.ExpectedDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new PurchaseOrderItemDto
                {
                    Id = oi.Id,
                    PurchaseOrderId = oi.PurchaseOrderId,
                    ShoeColorVariationId = oi.ShoeColorVariationId,
                    ShoeName = oi.ShoeColorVariation.Shoe.Name,
                    ColorName = oi.ShoeColorVariation.ColorName,
                    QuantityOrdered = oi.QuantityOrdered,
                    QuantityReceived = oi.QuantityReceived,
                    UnitCost = oi.UnitCost
                }).ToList()
            };
        }
    }
}