using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn_DangKyTourDuLich.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name = "Xếp hạng")]
        [Range(1, 5, ErrorMessage = "Xếp hạng từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [Display(Name = "Bình luận")]
        [StringLength(1000)]
        public string? Comment { get; set; }

        [Display(Name = "Ngày đánh giá")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
