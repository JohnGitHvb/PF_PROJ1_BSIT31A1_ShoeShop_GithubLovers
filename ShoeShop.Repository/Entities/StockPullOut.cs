using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Entities
{
    public enum PullOutStatus
    {
        Pending,
        Approved,
        Completed,
        Rejected
    }

    public class StockPullOut
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShoeColorVariationId { get; set; }

        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ReasonDetails { get; set; }

        [Required]
        [MaxLength(100)]
        public string RequestedBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        public DateTime PullOutDate { get; set; } = DateTime.Now;

        public PullOutStatus Status { get; set; } = PullOutStatus.Pending;

        // Navigation property
        [ForeignKey("ShoeColorVariationId")]
        public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
    }
}