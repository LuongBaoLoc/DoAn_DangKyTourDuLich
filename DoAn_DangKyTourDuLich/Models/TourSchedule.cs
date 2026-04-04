using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn_DangKyTourDuLich.Models
{
    public class TourSchedule
    {
        [Key]
        public int Id { get; set; }

        public int TourId { get; set; }

        [Required]
        [Display(Name = "Ngày khởi hành")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Giá người lớn")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; } // Nếu khác 0 sẽ dùng giá này, ngược lại lấy từ Tour

        [Display(Name = "Số người tối đa")]
        [Range(1, 1000)]
        public int MaxParticipants { get; set; } = 30;

        [Display(Name = "Số người đã đăng ký")]
        public int CurrentParticipants { get; set; } = 0;

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("TourId")]
        public virtual Tour? Tour { get; set; }

        [NotMapped]
        public int AvailableSlots => MaxParticipants - CurrentParticipants;
    }
}
