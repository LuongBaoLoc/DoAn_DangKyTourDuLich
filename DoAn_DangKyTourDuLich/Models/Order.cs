using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn_DangKyTourDuLich.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending = 0,

        [Display(Name = "Đã xác nhận")]
        Confirmed = 1,

        [Display(Name = "Đã hoàn thành")]
        Completed = 2,

        [Display(Name = "Đã hủy")]
        Cancelled = 3
    }

    public enum PaymentMethod
    {
        [Display(Name = "Thanh toán khi nhận tour")]
        CashOnDelivery = 0,

        [Display(Name = "Chuyển khoản ngân hàng")]
        BankTransfer = 1,

        [Display(Name = "Thanh toán online")]
        OnlinePayment = 2
    }

    public class Order
    {
        public int Id { get; set; }

        [Display(Name = "Mã đơn hàng")]
        [StringLength(20)]
        public string OrderCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên người đặt")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string? CustomerAddress { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Note { get; set; }

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

        [Display(Name = "Ngày đặt")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Đã được đánh giá")]
        public bool IsReviewed { get; set; } = false;

        // Foreign key
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
