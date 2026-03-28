using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoAn_DangKyTourDuLich.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
