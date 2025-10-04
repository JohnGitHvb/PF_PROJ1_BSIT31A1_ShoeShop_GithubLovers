using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ShoeShopDbContext _context;

        public InventoryService(ShoeShopDbContext context)
        {
            _context = context;
        }

        // Shoe Management
        public async Task<List<ShoeDto>> GetAllShoesAsync()
        {
            var shoes = await _context.Shoes
                .Include(s => s.ColorVariations)
                .Where(s => s.IsActive)
                .ToListAsync();

            return shoes.Select(s => new ShoeDto
            {
                Id = s.Id,
                Name = s.Name,
                Brand = s.Brand,
                Cost = s.Cost,
                Price = s.Price,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                IsActive = s.IsActive,
                CreatedDate = s.CreatedDate,
                ColorVariations = s.ColorVariations.Select(cv => new ShoeColorVariationDto
                {
                    Id = cv.Id,
                    ShoeId = cv.ShoeId,
                    ShoeName = s.Name,
                    Brand = s.Brand,
                    ColorName = cv.ColorName,
                    HexCode = cv.HexCode,
                    StockQuantity = cv.StockQuantity,
                    ReorderLevel = cv.ReorderLevel,
                    IsActive = cv.IsActive
                }).ToList()
            }).ToList();
        }

        public async Task<ShoeDto?> GetShoeByIdAsync(int id)
        {
            var shoe = await _context.Shoes
                .Include(s => s.ColorVariations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null) return null;

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
                CreatedDate = shoe.CreatedDate,
                ColorVariations = shoe.ColorVariations.Select(cv => new ShoeColorVariationDto
                {
                    Id = cv.Id,
                    ShoeId = cv.ShoeId,
                    ShoeName = shoe.Name,
                    Brand = shoe.Brand,
                    ColorName = cv.ColorName,
                    HexCode = cv.HexCode,
                    StockQuantity = cv.StockQuantity,
                    ReorderLevel = cv.ReorderLevel,
                    IsActive = cv.IsActive
                }).ToList()
            };
        }

        public async Task<ShoeDto> CreateShoeAsync(CreateShoeDto dto)
        {
            var shoe = new Shoe
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Cost = dto.Cost,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Shoes.Add(shoe);
            await _context.SaveChangesAsync();

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

        public async Task<ShoeDto> UpdateShoeAsync(int id, CreateShoeDto dto)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe == null)
                throw new Exception($"Shoe with ID {id} not found");

            shoe.Name = dto.Name;
            shoe.Brand = dto.Brand;
            shoe.Cost = dto.Cost;
            shoe.Price = dto.Price;
            shoe.Description = dto.Description;
            shoe.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();

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

        public async Task<bool> DeleteShoeAsync(int id)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe == null) return false;

            shoe.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Color Variation Management
        public async Task<List<ShoeColorVariationDto>> GetAllColorVariationsAsync()
        {
            var variations = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive)
                .ToListAsync();

            return variations.Select(cv => new ShoeColorVariationDto
            {
                Id = cv.Id,
                ShoeId = cv.ShoeId,
                ShoeName = cv.Shoe.Name,
                Brand = cv.Shoe.Brand,
                ColorName = cv.ColorName,
                HexCode = cv.HexCode,
                StockQuantity = cv.StockQuantity,
                ReorderLevel = cv.ReorderLevel,
                IsActive = cv.IsActive
            }).ToList();
        }

        public async Task<List<ShoeColorVariationDto>> GetColorVariationsByShoeIdAsync(int shoeId)
        {
            var variations = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.ShoeId == shoeId && cv.IsActive)
                .ToListAsync();

            return variations.Select(cv => new ShoeColorVariationDto
            {
                Id = cv.Id,
                ShoeId = cv.ShoeId,
                ShoeName = cv.Shoe.Name,
                Brand = cv.Shoe.Brand,
                ColorName = cv.ColorName,
                HexCode = cv.HexCode,
                StockQuantity = cv.StockQuantity,
                ReorderLevel = cv.ReorderLevel,
                IsActive = cv.IsActive
            }).ToList();
        }

        public async Task<ShoeColorVariationDto?> GetColorVariationByIdAsync(int id)
        {
            var variation = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .FirstOrDefaultAsync(cv => cv.Id == id);

            if (variation == null) return null;

            return new ShoeColorVariationDto
            {
                Id = variation.Id,
                ShoeId = variation.ShoeId,
                ShoeName = variation.Shoe.Name,
                Brand = variation.Shoe.Brand,
                ColorName = variation.ColorName,
                HexCode = variation.HexCode,
                StockQuantity = variation.StockQuantity,
                ReorderLevel = variation.ReorderLevel,
                IsActive = variation.IsActive
            };
        }

        public async Task<ShoeColorVariationDto> CreateColorVariationAsync(CreateShoeColorVariationDto dto)
        {
            var shoe = await _context.Shoes.FindAsync(dto.ShoeId);
            if (shoe == null)
                throw new Exception($"Shoe with ID {dto.ShoeId} not found");

            var variation = new ShoeColorVariation
            {
                ShoeId = dto.ShoeId,
                ColorName = dto.ColorName,
                HexCode = dto.HexCode,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel,
                IsActive = true
            };

            _context.ShoeColorVariations.Add(variation);
            await _context.SaveChangesAsync();

            return new ShoeColorVariationDto
            {
                Id = variation.Id,
                ShoeId = variation.ShoeId,
                ShoeName = shoe.Name,
                Brand = shoe.Brand,
                ColorName = variation.ColorName,
                HexCode = variation.HexCode,
                StockQuantity = variation.StockQuantity,
                ReorderLevel = variation.ReorderLevel,
                IsActive = variation.IsActive
            };
        }

        public async Task<ShoeColorVariationDto> UpdateColorVariationAsync(int id, CreateShoeColorVariationDto dto)
        {
            var variation = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .FirstOrDefaultAsync(cv => cv.Id == id);

            if (variation == null)
                throw new Exception($"Color variation with ID {id} not found");

            variation.ShoeId = dto.ShoeId;
            variation.ColorName = dto.ColorName;
            variation.HexCode = dto.HexCode;
            variation.StockQuantity = dto.StockQuantity;
            variation.ReorderLevel = dto.ReorderLevel;

            await _context.SaveChangesAsync();

            return new ShoeColorVariationDto
            {
                Id = variation.Id,
                ShoeId = variation.ShoeId,
                ShoeName = variation.Shoe.Name,
                Brand = variation.Shoe.Brand,
                ColorName = variation.ColorName,
                HexCode = variation.HexCode,
                StockQuantity = variation.StockQuantity,
                ReorderLevel = variation.ReorderLevel,
                IsActive = variation.IsActive
            };
        }

        public async Task<bool> DeleteColorVariationAsync(int id)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(id);
            if (variation == null) return false;

            variation.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Stock Management
        public async Task<bool> AdjustStockAsync(int colorVariationId, int quantity, string reason)
        {
            var variation = await _context.ShoeColorVariations.FindAsync(colorVariationId);
            if (variation == null) return false;

            variation.StockQuantity += quantity;

            if (variation.StockQuantity < 0)
                throw new Exception("Stock quantity cannot be negative");

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ShoeColorVariationDto>> GetLowStockItemsAsync()
        {
            var variations = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive && cv.StockQuantity <= cv.ReorderLevel)
                .ToListAsync();

            return variations.Select(cv => new ShoeColorVariationDto
            {
                Id = cv.Id,
                ShoeId = cv.ShoeId,
                ShoeName = cv.Shoe.Name,
                Brand = cv.Shoe.Brand,
                ColorName = cv.ColorName,
                HexCode = cv.HexCode,
                StockQuantity = cv.StockQuantity,
                ReorderLevel = cv.ReorderLevel,
                IsActive = cv.IsActive
            }).ToList();
        }

        public async Task<int> GetTotalStockCountAsync()
        {
            return await _context.ShoeColorVariations
                .Where(cv => cv.IsActive)
                .SumAsync(cv => cv.StockQuantity);
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var total = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .Where(cv => cv.IsActive)
                .SumAsync(cv => cv.StockQuantity * cv.Shoe.Cost);

            return total;
        }
    }
}