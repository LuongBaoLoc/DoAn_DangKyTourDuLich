using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn_DangKyTourDuLich.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Display(Name = "Số lượng người")]
        [Range(1, 100, ErrorMessage = "Số lượng người phải từ 1 đến 100")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Đơn giá")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Thành tiền")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal SubTotal { get; set; }

        // Foreign keys
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public virtual Tour? Tour { get; set; }
    }
}
