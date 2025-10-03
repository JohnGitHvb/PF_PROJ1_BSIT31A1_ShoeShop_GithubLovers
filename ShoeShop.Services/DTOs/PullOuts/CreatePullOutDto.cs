using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs.PullOuts
{
    public class CreatePullOutDto
    {
        [Required]
        public int ShoeColorVariationId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, StringLength(100)]
        public string Reason { get; set; }

        [StringLength(2000)]
        public string ReasonDetails { get; set; }

        [Required, StringLength(100)]
        public string RequestedBy { get; set; }
    }
}
