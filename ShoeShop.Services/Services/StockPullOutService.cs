using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class StockPullOutService : IStockPullOutService
    {
        private readonly ShoeShopDbContext _context;

        public StockPullOutService(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockPullOutDto>> GetAllPullOutsAsync()
        {
            var pullOuts = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                    .ThenInclude(cv => cv.Shoe)
                .OrderByDescending(spo => spo.PullOutDate)
                .ToListAsync();

            return pullOuts.Select(MapToDto).ToList();
        }

        public async Task<List<StockPullOutDto>> GetPullOutsByStatusAsync(PullOutStatus status)
        {
            var pullOuts = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                    .ThenInclude(cv => cv.Shoe)
                .Where(spo => spo.Status == status)
                .OrderByDescending(spo => spo.PullOutDate)
                .ToListAsync();

            return pullOuts.Select(MapToDto).ToList();
        }

        public async Task<StockPullOutDto?> GetPullOutByIdAsync(int id)
        {
            var pullOut = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                    .ThenInclude(cv => cv.Shoe)
                .FirstOrDefaultAsync(spo => spo.Id == id);

            return pullOut == null ? null : MapToDto(pullOut);
        }

        public async Task<StockPullOutDto> CreatePullOutAsync(CreateStockPullOutDto dto)
        {
            // Validate color variation exists
            var colorVariation = await _context.ShoeColorVariations
                .Include(cv => cv.Shoe)
                .FirstOrDefaultAsync(cv => cv.Id == dto.ShoeColorVariationId);

            if (colorVariation == null)
                throw new Exception($"Shoe color variation with ID {dto.ShoeColorVariationId} not found");

            // Check if sufficient stock is available
            if (colorVariation.StockQuantity < dto.Quantity)
            {
                throw new Exception($"Insufficient stock. Available: {colorVariation.StockQuantity}, Requested: {dto.Quantity}");
            }

            var pullOut = new StockPullOut
            {
                ShoeColorVariationId = dto.ShoeColorVariationId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                ReasonDetails = dto.ReasonDetails,
                RequestedBy = dto.RequestedBy,
                PullOutDate = DateTime.Now,
                Status = PullOutStatus.Pending
            };

            _context.StockPullOuts.Add(pullOut);
            await _context.SaveChangesAsync();

            // Reload with includes
            return (await GetPullOutByIdAsync(pullOut.Id))!;
        }

        public async Task<bool> ApprovePullOutAsync(int pullOutId, string approvedBy)
        {
            var pullOut = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                .FirstOrDefaultAsync(spo => spo.Id == pullOutId);

            if (pullOut == null) return false;

            if (pullOut.Status != PullOutStatus.Pending)
                throw new Exception("Only pending pull-outs can be approved");

            // Check stock availability again
            if (pullOut.ShoeColorVariation.StockQuantity < pullOut.Quantity)
            {
                throw new Exception($"Insufficient stock for approval. Available: {pullOut.ShoeColorVariation.StockQuantity}, Required: {pullOut.Quantity}");
            }

            pullOut.Status = PullOutStatus.Approved;
            pullOut.ApprovedBy = approvedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPullOutAsync(int pullOutId, string reason)
        {
            var pullOut = await _context.StockPullOuts.FindAsync(pullOutId);
            if (pullOut == null) return false;

            if (pullOut.Status != PullOutStatus.Pending)
                throw new Exception("Only pending pull-outs can be rejected");

            pullOut.Status = PullOutStatus.Rejected;
            pullOut.ReasonDetails = $"Rejected: {reason}";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletePullOutAsync(int pullOutId)
        {
            var pullOut = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                .FirstOrDefaultAsync(spo => spo.Id == pullOutId);

            if (pullOut == null) return false;

            if (pullOut.Status != PullOutStatus.Approved)
                throw new Exception("Only approved pull-outs can be completed");

            // Deduct stock
            pullOut.ShoeColorVariation.StockQuantity -= pullOut.Quantity;

            if (pullOut.ShoeColorVariation.StockQuantity < 0)
            {
                throw new Exception("Stock quantity cannot be negative");
            }

            pullOut.Status = PullOutStatus.Completed;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingPullOutsCountAsync()
        {
            return await _context.StockPullOuts
                .CountAsync(spo => spo.Status == PullOutStatus.Pending);
        }

        public async Task<List<StockPullOutDto>> GetPullOutsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var pullOuts = await _context.StockPullOuts
                .Include(spo => spo.ShoeColorVariation)
                    .ThenInclude(cv => cv.Shoe)
                .Where(spo => spo.PullOutDate >= startDate && spo.PullOutDate <= endDate)
                .OrderByDescending(spo => spo.PullOutDate)
                .ToListAsync();

            return pullOuts.Select(MapToDto).ToList();
        }

        // Helper method
        private StockPullOutDto MapToDto(StockPullOut pullOut)
        {
            return new StockPullOutDto
            {
                Id = pullOut.Id,
                ShoeColorVariationId = pullOut.ShoeColorVariationId,
                ShoeName = pullOut.ShoeColorVariation.Shoe.Name,
                ColorName = pullOut.ShoeColorVariation.ColorName,
                Quantity = pullOut.Quantity,
                Reason = pullOut.Reason,
                ReasonDetails = pullOut.ReasonDetails,
                RequestedBy = pullOut.RequestedBy,
                ApprovedBy = pullOut.ApprovedBy,
                PullOutDate = pullOut.PullOutDate,
                Status = pullOut.Status
            };
        }
    }
}