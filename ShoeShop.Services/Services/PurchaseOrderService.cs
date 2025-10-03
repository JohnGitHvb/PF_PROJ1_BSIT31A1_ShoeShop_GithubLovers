using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Services.DTOs.PurchaseOrders;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.Enums;
using ShoeShop;
using ShoeShop.Data;
using ShoeShop.Entities;

namespace ShoeShop.Services.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ShoeShopDbContext _db;
        private readonly IInventoryService _inventory;
        public PurchaseOrderService(ShoeShopDbContext db, IInventoryService inventory)
        {
            _db = db;
            _inventory = inventory;
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Purchase order must contain items.");

            var supplier = await _db.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null) throw new KeyNotFoundException("Supplier not found.");

            var po = new PurchaseOrder
            {
                OrderNumber = dto.OrderNumber,
                SupplierId = dto.SupplierId,
                OrderDate = dto.OrderDate,
                ExpectedDate = (DateTime)dto.ExpectedDate,
                Status = Entities.PurchaseOrderStatus.Pending,
                TotalAmount = dto.Items.Sum(i => i.QuantityOrdered * i.UnitCost)
            };
            _db.PurchaseOrders.Add(po);
            await _db.SaveChangesAsync();

            foreach (var it in dto.Items)
            {
                var variation = await _db.ShoeColorVariations.FindAsync(it.ShoeColorVariationId);
                if (variation == null) throw new KeyNotFoundException($"Variation {it.ShoeColorVariationId} not found.");

                var poi = new PurchaseOrderItem
                {
                    PurchaseOrderId = po.Id,
                    ShoeColorVariationId = it.ShoeColorVariationId,
                    QuantityOrdered = it.QuantityOrdered,
                    QuantityReceived = 0,
                    UnitCost = it.UnitCost
                };
                _db.PurchaseOrderItems.Add(poi);
            }

            await _db.SaveChangesAsync();
            return await GetPurchaseOrderAsync(po.Id);
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderAsync(int id)
        {
            var po = await _db.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Items).ThenInclude(i => i.ShoeColorVariation).ThenInclude(v => v.Shoe)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (po == null) return null;

            var dto = new PurchaseOrderDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                SupplierId = po.SupplierId,
                SupplierName = po.Supplier?.Name,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                Status = (Enums.PurchaseOrderStatus)po.Status,
                TotalAmount = po.TotalAmount
            };

            dto.Items = po.Items.Select(i => new PurchaseOrderItemDto
            {
                Id = i.Id,
                ShoeColorVariationId = i.ShoeColorVariationId,
                ColorName = i.ShoeColorVariation?.ColorName,
                QuantityOrdered = i.QuantityOrdered,
                QuantityReceived = i.QuantityReceived,
                UnitCost = i.UnitCost
            }).ToList();

            return dto;
        }

        public async Task ReceivePurchaseOrderItemAsync(int purchaseOrderItemId, int quantityReceived, Entities.PurchaseOrderStatus purchaseOrderStatus)
        {
            if (quantityReceived <= 0) throw new ArgumentException("Quantity must be positive.");

            var item = await _db.PurchaseOrderItems
                .Include(i => i.PurchaseOrder)
                .FirstOrDefaultAsync(i => i.Id == purchaseOrderItemId);
            if (item == null) throw new KeyNotFoundException("Purchase order item not found.");

            var remaining = item.QuantityOrdered - item.QuantityReceived;
            if (quantityReceived > remaining)
                throw new InvalidOperationException("Quantity received exceeds quantity ordered.");

            item.QuantityReceived += quantityReceived;
            _db.PurchaseOrderItems.Update(item);

            await _inventory.AddStockAsync(item.ShoeColorVariationId, quantityReceived, $"Received from PO {item.PurchaseOrder.OrderNumber}");

            var po = await _db.PurchaseOrders.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == item.PurchaseOrderId);
            if (po.Items.All(i => i.QuantityReceived >= i.QuantityOrdered))
                po.Status = Entities.PurchaseOrderStatus.Received;
            else
                po.Status = Entities.PurchaseOrderStatus.Shipped;

            _db.PurchaseOrders.Update(po);
            await _db.SaveChangesAsync();
        }

        public async Task ConfirmPurchaseOrderAsync(int purchaseOrderId)
        {
            var po = await _db.PurchaseOrders.FindAsync(purchaseOrderId);
            if (po == null) throw new KeyNotFoundException("Purchase order not found.");
            if (po.Status != Entities.PurchaseOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be confirmed.");

            po.Status = Entities.PurchaseOrderStatus.Confirmed;
            _db.PurchaseOrders.Update(po);
            await _db.SaveChangesAsync();
        }

        Task<PurchaseOrderDto> IPurchaseOrderService.CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            throw new NotImplementedException();
        }

        Task<PurchaseOrderDto> IPurchaseOrderService.GetPurchaseOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task IPurchaseOrderService.ReceivePurchaseOrderItemAsync(int purchaseOrderItemId, int quantityReceived)
        {
            throw new NotImplementedException();
        }

        Task IPurchaseOrderService.ConfirmPurchaseOrderAsync(int purchaseOrderId)
        {
            throw new NotImplementedException();
        }
    }
}
