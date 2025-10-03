using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Services.DTOs.PullOuts;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.Enums;
using ShoeShop;
using ShoeShop.Data;
using ShoeShop.Entities;

namespace ShoeShop.Services.Services
{
    public class PullOutService : IPullOutService
    {
        private readonly ShoeShopDbContext _db;
        private readonly IInventoryService _inventory;
        public PullOutService(ShoeShopDbContext db, IInventoryService inventory)
        {
            _db = db; _inventory = inventory;
        }

        public PullOutStatus GetPullOutStatus()
        {
            return PullOutStatus.Pending;
        }

        public async Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto dto, PullOutStatus pullOutStatus)
        {
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            var variation = await _db.ShoeColorVariations.FindAsync(dto.ShoeColorVariationId);
            if (variation == null) throw new KeyNotFoundException("Variation not found.");

            if (variation.StockQuantity < dto.Quantity) throw new InvalidOperationException("Insufficient stock for pull-out.");

            var pull = new StockPullOut
            {
                ShoeColorVariationId = dto.ShoeColorVariationId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                ReasonDetails = dto.ReasonDetails,
                RequestedBy = dto.RequestedBy,
                ApprovedBy = null,
                PullOutDate = DateTime.UtcNow,
                Status = (StockPullOutStatus)pullOutStatus 
            };

            _db.StockPullOuts.Add(pull);
            await _db.SaveChangesAsync();

            return MapToDto(pull);
        }

        public async Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto dto)
        {
            return await CreatePullOutRequestAsync(dto, PullOutStatus.Pending);
        }

        public async Task<IEnumerable<PullOutRequestDto>> GetPendingPullOutsAsync()
        {
            return await _db.StockPullOuts
                .Where(p => p.Status == StockPullOutStatus.Pending)
                .Include(p => p.ShoeColorVariation).ThenInclude(v => v.Shoe)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<PullOutRequestDto> ApprovePullOutAsync(int pullOutId, string approverRole, string approverName)
        {
            var p = await _db.StockPullOuts.Include(i => i.ShoeColorVariation).FirstOrDefaultAsync(x => x.Id == pullOutId);
            if (p == null) throw new KeyNotFoundException("Pull-out request not found.");
            if (p.Status != StockPullOutStatus.Pending) throw new InvalidOperationException("Only pending requests can be approved.");

            if (p.Quantity > 10 && (approverRole == null || !(approverRole.Equals("manager", StringComparison.OrdinalIgnoreCase) || approverRole.Equals("admin", StringComparison.OrdinalIgnoreCase))))
            {
                throw new InvalidOperationException("This request requires Manager or Admin approval.");
            }

            p.Status = StockPullOutStatus.Approved;
            p.ApprovedBy = approverName;
            _db.StockPullOuts.Update(p);
            await _db.SaveChangesAsync();

            return MapToDto(p);
        }

        public async Task<PullOutRequestDto> CompletePullOutAsync(int pullOutId)
        {
            var p = await _db.StockPullOuts.Include(i => i.ShoeColorVariation).FirstOrDefaultAsync(x => x.Id == pullOutId);
            if (p == null) throw new KeyNotFoundException("Pull-out request not found.");
            if (p.Status != StockPullOutStatus.Approved) throw new InvalidOperationException("Only approved requests can be completed.");

            await _inventory.RemoveStockAsync(p.ShoeColorVariationId, p.Quantity, $"Pull-out completed #{p.Id}");

            p.Status = StockPullOutStatus.Completed;
            _db.StockPullOuts.Update(p);
            await _db.SaveChangesAsync();

            return MapToDto(p);
        }

        public async Task RejectPullOutAsync(int pullOutId, string approverName, string reason)
        {
            var p = await _db.StockPullOuts.FindAsync(pullOutId);
            if (p == null) throw new KeyNotFoundException("Pull-out request not found.");
            if (p.Status != StockPullOutStatus.Pending) throw new InvalidOperationException("Only pending requests can be rejected.");

            p.Status = StockPullOutStatus.Rejected;
            p.ApprovedBy = approverName;
            p.ReasonDetails = (p.ReasonDetails ?? "") + $"\nRejectedReason: {reason}";
            _db.StockPullOuts.Update(p);
            await _db.SaveChangesAsync();
        }

        private PullOutRequestDto MapToDto(StockPullOut p)
        {
            return new PullOutRequestDto
            {
                Id = p.Id,
                ShoeColorVariationId = p.ShoeColorVariationId,
                ColorName = p.ShoeColorVariation?.ColorName,
                Quantity = p.Quantity,
                Reason = p.Reason,
                ReasonDetails = p.ReasonDetails,
                RequestedBy = p.RequestedBy,
                ApprovedBy = p.ApprovedBy,
                PullOutDate = p.PullOutDate,
                Status = (PullOutStatus)p.Status
            };
        }
    }
}
