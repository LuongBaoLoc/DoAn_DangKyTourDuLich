using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DoAn_DangKyTourDuLich.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name = "Người đánh giá")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Display(Name = "Tour được đánh giá")]
        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public virtual Tour? Tour { get; set; }

        [Display(Name = "Đơn hàng")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Order? Booking { get; set; }

        [Display(Name = "Số sao")]
        [Range(1, 5, ErrorMessage = "Vui lòng chọn từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [Display(Name = "Nhận xét")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nhận xét phải từ 10 đến 1000 ký tự")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "URL ảnh (JSON)")]
        public string? ImagesData { get; set; }

        [Display(Name = "Bị ẩn")]
        public bool IsHidden { get; set; } = false;

        [Display(Name = "Lý do ẩn")]
        [StringLength(500)]
        public string? HideReason { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Helper method to get images list from JSON
        [NotMapped]
        public List<string> ImageUrls
        {
            get
            {
                if (string.IsNullOrEmpty(ImagesData))
                    return new List<string>();
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(ImagesData) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }
            set
            {
                ImagesData = JsonSerializer.Serialize(value ?? new List<string>());
            }
        }
    }
}

