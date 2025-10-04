using ShoeShop.Repository.Entities;

namespace ShoeShop.Services.DTOs
{
    public class StockPullOutDto
    {
        public int Id { get; set; }
        public int ShoeColorVariationId { get; set; }
        public string ShoeName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetails { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime PullOutDate { get; set; }
        public PullOutStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
    }
}