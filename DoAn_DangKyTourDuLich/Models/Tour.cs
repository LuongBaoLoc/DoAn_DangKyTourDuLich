using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DoAn_DangKyTourDuLich.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mã tour")]
        [StringLength(50)]
        public string? TourCode { get; set; }

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
        [Display(Name = "Giá gốc (VNĐ)")]
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

        [Display(Name = "Danh sách hình ảnh (JSON)")]
        public string? ImageUrlsData { get; set; }

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

        // --- KHÓA NGOẠI ---
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // --- LIÊN KẾT BẢNG ---
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // --- THUỘC TÍNH TÍNH TOÁN (KHÔNG LƯU DB) ---
        [NotMapped]
        [Display(Name = "Chỗ còn trống")]
        public int AvailableSlots => MaxParticipants - CurrentParticipants;

        [NotMapped]
        [Display(Name = "Giá hiển thị")]
        public decimal DisplayPrice => DiscountPrice ?? Price;

        [NotMapped]
        public List<string> GalleryImages
        {
            get
            {
                var images = new List<string>();
                if (!string.IsNullOrWhiteSpace(ImageUrl)) images.Add(ImageUrl);
                if (string.IsNullOrWhiteSpace(ImageUrlsData)) return images;
                try
                {
                    var storedImages = JsonSerializer.Deserialize<List<string>>(ImageUrlsData);
                    if (storedImages != null)
                    {
                        foreach (var img in storedImages)
                        {
                            if (!string.IsNullOrWhiteSpace(img) && !images.Contains(img))
                                images.Add(img);
                        }
                    }
                }
                catch
                {
                    var splitImages = ImageUrlsData.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var img in splitImages)
                    {
                        if (!images.Contains(img)) images.Add(img);
                    }
                }
                return images;
            }
        }

        public void SetGalleryImages(IEnumerable<string?> imageUrls)
        {
            var images = imageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => url!.Trim())
                .Distinct()
                .ToList();

            ImageUrl = images.FirstOrDefault();
            ImageUrlsData = images.Count == 0 ? null : JsonSerializer.Serialize(images);
        }

        // --- THUỘC TÍNH PHỤC VỤ THUẬT TOÁN KHẢO SÁT ---
        [Display(Name = "Tour Biển")]
        public bool IsBeach { get; set; }

        [Display(Name = "Tour Núi")]
        public bool IsMountain { get; set; }

        [Display(Name = "Phù hợp đi Nhóm")]
        public bool IsForGroup { get; set; }

        [Display(Name = "Tour Tiết Kiệm")]
        public bool IsLowBudget { get; set; }
    }
}