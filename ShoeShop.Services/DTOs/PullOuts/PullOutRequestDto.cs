using System;
using ShoeShop.Services.Enums;

namespace ShoeShop.Services.DTOs.PullOuts
{
    public class PullOutRequestDto
    {
        public int Id { get; set; }
        public int ShoeColorVariationId { get; set; }
        public string ColorName { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string ReasonDetails { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime PullOutDate { get; set; }
        public PullOutStatus Status { get; set; }
    }
}
