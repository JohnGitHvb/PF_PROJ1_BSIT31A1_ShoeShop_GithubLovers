using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Services.DTOs.Shoes;
using ShoeShop.Services.DTOs.Reports;
using ShoeShop.Services.Interfaces;
using ShoeShop;
using ShoeShop.Data;
using ShoeShop.Entities;

namespace ShoeShop.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ShoeShopDbContext _db;
        public InventoryService(ShoeShopDbContext db) => _db = db;

        public async Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto)
        {
            if (dto.Price < dto.Cost)
                throw new InvalidOperationException("Selling Price should not be less than Cost.");

            var shoe = new Shoe
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Cost = dto.Cost,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.UtcNow
            };

            _db.Shoes.Add(shoe);
            await _db.SaveChangesAsync();

            return new ShoeDto
            {
                Id = shoe.Id,
                Name = shoe.Name,
                Brand = shoe.Brand,
                Cost = shoe.Cost,
                Price = shoe.Price,
                Description = shoe.Description,
                ImageUrl = shoe.ImageUrl,
                IsActive = shoe.IsActive,
                CreatedDate = shoe.CreatedDate
            };
        }

        public async Task<ShoeDto> GetShoeAsync(int id)
        {
            var s = await _db.Shoes.FindAsync(id);
            if (s == null) return null;
            return new ShoeDto
            {
                Id = s.Id,
                Name = s.Name,
                Brand = s.Brand,
                Cost = s.Cost,
                Price = s.Price,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                IsActive = s.IsActive,
                CreatedDate = s.CreatedDate
            };
        }

        public async Task<IEnumerable<ShoeDto>> GetAllShoesAsync()
        {
            return await _db.Shoes
                .OrderBy(s => s.Name)
                .Select(s => new ShoeDto {
                    Id = s.Id, Name = s.Name, Brand = s.Brand, Cost = s.Cost,
                    Price = s.Price, Description = s.Description, ImageUrl = s.ImageUrl,
                    IsActive = s.IsActive, CreatedDate = s.CreatedDate
                })
                .ToListAsync();
        }

        public async Task AddStockAsync(int shoeColorVariationId, int quantity, string note = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            var v = await _db.ShoeColorVariations.FindAsync(shoeColorVariationId);
            if (v == null) throw new KeyNotFoundException("Variation not found.");
            v.StockQuantity += quantity;
            _db.ShoeColorVariations.Update(v);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveStockAsync(int shoeColorVariationId, int quantity, string note = null)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            var v = await _db.ShoeColorVariations.FindAsync(shoeColorVariationId);
            if (v == null) throw new KeyNotFoundException("Variation not found.");
            if (v.StockQuantity < quantity) throw new InvalidOperationException("Insufficient stock.");
            v.StockQuantity -= quantity;
            _db.ShoeColorVariations.Update(v);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoeColorVariationLowStockDto>> GetLowStockAsync(int threshold = 5)
        {
            return await _db.ShoeColorVariations
                .Where(v => v.IsActive && v.StockQuantity <= (v.ReorderLevel == 0 ? threshold : v.ReorderLevel))
                .Include(v => v.Shoe)
                .Select(v => new ShoeColorVariationLowStockDto
                {
                    ShoeColorVariationId = v.Id,
                    ShoeName = v.Shoe.Name,
                    ColorName = v.ColorName,
                    StockQuantity = v.StockQuantity,
                    ReorderLevel = v.ReorderLevel
                })
                .ToListAsync();
        }

        public async Task<InventoryReportDto> GetInventoryReportAsync()
        {
            var variations = await _db.ShoeColorVariations.Include(v => v.Shoe).ToListAsync();
            var totalSku = variations.Count;
            var totalUnits = variations.Sum(v => v.StockQuantity);
            var totalValue = variations.Sum(v => v.StockQuantity * v.Shoe.Cost);
            var lowStockCount = variations.Count(v => v.StockQuantity <= v.ReorderLevel);

            return new InventoryReportDto
            {
                TotalSkuCount = totalSku,
                TotalUnitsInStock = totalUnits,
                TotalInventoryValue = totalValue,
                LowStockCount = lowStockCount
            };
        }
    }
}
