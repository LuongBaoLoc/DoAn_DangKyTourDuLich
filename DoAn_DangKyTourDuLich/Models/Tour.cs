using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn_DangKyTourDuLich.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tour")]
        [Display(Name = "Tên tour")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [Display(Name = "Mô tả ngắn")]
        [StringLength(500)]
        public string ShortDescription { get; set; } = string.Empty;

        [Display(Name = "Mô tả chi tiết")]
        public string? DetailDescription { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Display(Name = "Giá (VNĐ)")]
        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Giá khuyến mãi")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm khởi hành")]
        [Display(Name = "Điểm khởi hành")]
        [StringLength(100)]
        public string DepartureLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập điểm đến")]
        [Display(Name = "Điểm đến")]
        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập thời gian")]
        [Display(Name = "Thời gian (ngày)")]
        [Range(1, 365, ErrorMessage = "Thời gian từ 1 đến 365 ngày")]
        public int Duration { get; set; }

        [Display(Name = "Ngày khởi hành")]
        [DataType(DataType.Date)]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "Số người tối đa")]
        [Range(1, 1000)]
        public int MaxParticipants { get; set; } = 30;

        [Display(Name = "Số người đã đăng ký")]
        public int CurrentParticipants { get; set; } = 0;

        [Display(Name = "Hình ảnh đại diện")]
        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Phương tiện di chuyển")]
        [StringLength(100)]
        public string? Transportation { get; set; }

        [Display(Name = "Lịch trình")]
        public string? Schedule { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Computed property
        [NotMapped]
        public int AvailableSlots => MaxParticipants - CurrentParticipants;

        [NotMapped]
        public decimal DisplayPrice => DiscountPrice ?? Price;
    }
}
