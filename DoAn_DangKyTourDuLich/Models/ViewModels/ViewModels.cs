using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class TourSearchViewModel
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Destination { get; set; }
        public int? Duration { get; set; }
        public string? Transportation { get; set; }
        public string? SortBy { get; set; }
        public string? GroupSize { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public List<Tour> Tours { get; set; } = new List<Tour>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }

    public class TourRecommendationViewModel
    {
        public Tour Tour { get; set; } = null!;
        public double Score { get; set; }
        public bool SameCategory { get; set; }
        public bool SameDestination { get; set; }
        public bool SimilarPrice { get; set; }
        public List<string> Reasons { get; set; } = new List<string>();
    }

    public class TourDetailsViewModel
    {
        public Tour Tour { get; set; } = null!;
        public List<TourRecommendationViewModel> RelatedTours { get; set; } = new List<TourRecommendationViewModel>();
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên người đặt")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string? CustomerAddress { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Note { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; }

        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Display(Name = "Ngày khởi hành")]
        public string SelectedDate { get; set; } = string.Empty;

        [Display(Name = "Người lớn")]
        [Range(1, 100)]
        public int AdultQuantity { get; set; } = 1;

        [Display(Name = "Trẻ em (5-11 tuổi)")]
        public int ChildQuantity { get; set; } = 0;

        [Display(Name = "Trẻ nhỏ (2-5 tuổi)")]
        public int ToddlerQuantity { get; set; } = 0;

        [Display(Name = "Em bé (< 2 tuổi)")]
        public int InfantQuantity { get; set; } = 0;

        [Display(Name = "Yêu cầu tách đoàn riêng (Cho khách tập thể)")]
        public bool IsPrivateGroup { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tổng số lượng")]
        [Display(Name = "Tổng số lượng người")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Phụ thu phòng đơn")]
        public int SingleRoomQuantity { get; set; } = 0;

        [Display(Name = "Tổng tiền tạm tính")]
        public decimal TotalAmount { get; set; }
    }
}
