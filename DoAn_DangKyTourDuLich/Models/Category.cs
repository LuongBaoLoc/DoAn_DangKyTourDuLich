using System.ComponentModel.DataAnnotations;

namespace DoAn_DangKyTourDuLich.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "Hiển thị")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}
