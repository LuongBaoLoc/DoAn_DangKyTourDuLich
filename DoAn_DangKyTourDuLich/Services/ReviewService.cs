using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Services
{
    public class ReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Kiểm tra xem người dùng có thể đánh giá đơn hàng hay không
        /// Điều kiện:
        /// 1. Đơn hàng phải ở trạng thái Completed (Đã hoàn thành)
        /// 2. Ngày kết thúc tour phải <= hôm nay
        /// 3. Đơn hàng chưa bị đánh giá (IsReviewed == false)
        /// 4. Người dùng phải chính chủ của đơn hàng
        /// </summary>
        public async Task<CanReviewResult> CanReviewAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new CanReviewResult { CanReview = false, Reason = "Đơn hàng không tồn tại" };

            // Kiểm tra quyền sở hữu
            if (order.UserId != userId)
                return new CanReviewResult { CanReview = false, Reason = "Bạn không có quyền đánh giá đơn hàng này" };

            // Kiểm tra trạng thái đơn hàng
            if (order.Status != OrderStatus.Completed)
            {
                return new CanReviewResult 
                { 
                    CanReview = false, 
                    Reason = $"Bạn chỉ có thể đánh giá sau khi tour kết thúc. Trạng thái hiện tại: {GetStatusDisplayName(order.Status)}" 
                };
            }

            // Kiểm tra đã đánh giá chưa
            if (order.IsReviewed)
                return new CanReviewResult { CanReview = false, Reason = "Bạn đã đánh giá đơn hàng này rồi" };

            // Tính toán ngày kết thúc tour
            var tourEndDate = GetTourEndDate(order);
            if (tourEndDate == null)
                return new CanReviewResult { CanReview = false, Reason = "Không thể xác định ngày kết thúc tour" };

            // Kiểm tra xem tour đã kết thúc chưa
            if (tourEndDate.Value.Date > DateTime.Now.Date)
            {
                return new CanReviewResult 
                { 
                    CanReview = false, 
                    Reason = $"Tour sẽ kết thúc vào {tourEndDate.Value.Date:dd/MM/yyyy}. Bạn có thể đánh giá sau ngày này",
                    TourEndDate = tourEndDate.Value
                };
            }

            return new CanReviewResult { CanReview = true, TourEndDate = tourEndDate };
        }

        /// <summary>
        /// Lấy danh sách đơn hàng đã hoàn thành của người dùng với thông tin có thể đánh giá
        /// </summary>
        public async Task<List<OrderReviewInfo>> GetCompletedOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var result = new List<OrderReviewInfo>();

            foreach (var order in orders)
            {
                var tourEndDate = GetTourEndDate(order);
                var canReviewResult = await CanReviewAsync(order.Id, userId);
                
                var tourName = order.OrderDetails?.FirstOrDefault()?.Tour?.Name ?? "Tour không tìm thấy";

                result.Add(new OrderReviewInfo
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    TourName = tourName,
                    OrderDate = order.OrderDate,
                    TourEndDate = tourEndDate,
                    Status = order.Status,
                    CanReview = canReviewResult.CanReview,
                    CanReviewReason = canReviewResult.Reason,
                    HasReview = order.Reviews.Any(),
                    IsReviewed = order.IsReviewed
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy thông tin review của một đơn hàng
        /// </summary>
        public async Task<Review?> GetOrderReviewAsync(int orderId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Tour)
                .FirstOrDefaultAsync(r => r.BookingId == orderId && !r.IsHidden);
        }

        /// <summary>
        /// Lấy danh sách các review công khai của một tour
        /// </summary>
        public async Task<List<Review>> GetTourReviewsAsync(int tourId, int pageIndex = 0, int pageSize = 10)
        {
            return await _context.Reviews
                .Where(r => r.TourId == tourId && !r.IsHidden)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Tính toán ngày kết thúc tour từ ngày khởi hành + thời gian
        /// </summary>
        public DateTime? GetTourEndDate(Order order)
        {
            var tour = order.OrderDetails?.FirstOrDefault()?.Tour;
            if (tour?.DepartureDate == null || tour.Duration == 0)
                return null;

            return tour.DepartureDate.Value.AddDays(tour.Duration);
        }

        /// <summary>
        /// Lấy tên hiển thị của trạng thái đơn hàng
        /// </summary>
        private string GetStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Chờ xác nhận",
                OrderStatus.Confirmed => "Đã xác nhận",
                OrderStatus.Completed => "Đã hoàn thành",
                OrderStatus.Cancelled => "Đã hủy",
                _ => "Không xác định"
            };
        }
    }

    public class CanReviewResult
    {
        public bool CanReview { get; set; }
        public string? Reason { get; set; }
        public DateTime? TourEndDate { get; set; }
    }

    public class OrderReviewInfo
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string TourName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? TourEndDate { get; set; }
        public OrderStatus Status { get; set; }
        public bool CanReview { get; set; }
        public string? CanReviewReason { get; set; }
        public bool HasReview { get; set; }
        public bool IsReviewed { get; set; }
    }
}
