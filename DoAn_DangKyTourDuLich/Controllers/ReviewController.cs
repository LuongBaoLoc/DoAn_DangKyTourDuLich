using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ReviewService _reviewService;
        private readonly ProfanityFilterService _profanityFilter;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(
            ApplicationDbContext context,
            ReviewService reviewService,
            ProfanityFilterService profanityFilter,
            CloudinaryService cloudinaryService,
            ILogger<ReviewController> logger)
        {
            _context = context;
            _reviewService = reviewService;
            _profanityFilter = profanityFilter;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        // GET: /Review/MyOrders - Danh sách đơn hàng có thể đánh giá
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Account/Login");
            }

            var orderList = await _reviewService.GetCompletedOrdersAsync(userId);
            return View(orderList);
        }

        // GET: /Review/AddReview/5 - Form đánh giá
        public async Task<IActionResult> AddReview(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Account/Login");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng");

            // Kiểm tra quyền truy cập
            if (order.UserId != userId)
                return Forbid();

            // Kiểm tra xem có thể đánh giá không
            var canReview = await _reviewService.CanReviewAsync(id, userId);
            ViewBag.CanReview = canReview.CanReview;
            ViewBag.CanReviewReason = canReview.Reason;

            // Kiểm tra nếu đã có review
            var existingReview = order.Reviews?.FirstOrDefault();
            ViewBag.ExistingReview = existingReview;

            return View(order);
        }

        // POST: /Review/AddReview - Submit review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int orderId, int rating, string comment, IFormFileCollection? images)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Người dùng không được xác thực");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            // Kiểm tra quyền
            if (order.UserId != userId)
                return Forbid();

            // Xác thực
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Vui lòng chọn từ 1 đến 5 sao";
                return RedirectToAction(nameof(AddReview), new { id = orderId });
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Vui lòng nhập bình luận";
                return RedirectToAction(nameof(AddReview), new { id = orderId });
            }

            // Kiểm tra có thể đánh giá không
            var canReview = await _reviewService.CanReviewAsync(orderId, userId);
            if (!canReview.CanReview)
            {
                TempData["Error"] = canReview.Reason;
                return RedirectToAction(nameof(AddReview), new { id = orderId });
            }

            // Kiểm tra phản tìm từ cấm
            var filterResult = _profanityFilter.FilterContent(comment);
            if (!filterResult.IsClean)
            {
                TempData["Error"] = $"Nhận xét chứa nội dung không phù hợp: {filterResult.Reason}";
                return RedirectToAction(nameof(AddReview), new { id = orderId });
            }

            // Lấy tour ID
            var tourId = order.OrderDetails?.FirstOrDefault()?.TourId ?? 0;

            try
            {
                // Upload images nếu có
                var imageUrls = new List<string>();
                if (images != null && images.Count > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadMultipleImagesAsync(images);
                    if (uploadResult.IsSuccessful)
                    {
                        imageUrls = uploadResult.GetUploadedUrls();
                    }
                    else
                    {
                        _logger.LogWarning($"Lỗi upload ảnh cho review: {string.Join(", ", uploadResult.Errors)}");
                    }
                }

                // Kiểm tra nếu đã có review - cập nhật
                var existingReview = order.Reviews?.FirstOrDefault();
                if (existingReview != null)
                {
                    existingReview.Rating = rating;
                    existingReview.Comment = comment;
                    existingReview.ImageUrls = imageUrls;
                    existingReview.UpdatedAt = DateTime.Now;
                    existingReview.IsHidden = false; // Reset lại trạng thái ẩn nếu admin đã ẩn
                    existingReview.HideReason = null;

                    _context.Reviews.Update(existingReview);
                    TempData["Success"] = "Cập nhật đánh giá thành công! Cảm ơn bạn 😊";
                }
                else
                {
                    // Tạo review mới
                    var review = new Review
                    {
                        BookingId = orderId,
                        TourId = tourId,
                        UserId = userId,
                        Rating = rating,
                        Comment = comment,
                        ImageUrls = imageUrls,
                        CreatedAt = DateTime.Now,
                        IsHidden = false
                    };

                    _context.Reviews.Add(review);
                    TempData["Success"] = "Gửi đánh giá thành công! Cảm ơn bạn 😊";
                }

                // Đánh dấu order đã được review
                order.IsReviewed = true;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Success), new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu review");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại";
                return RedirectToAction(nameof(AddReview), new { id = orderId });
            }
        }

        // GET: /Review/Success/5 - Thành công
        public async Task<IActionResult> Success(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: /Review/TourReviews/5 - Danh sách review công khai của tour
        public async Task<IActionResult> TourReviews(int tourId, int pageIndex = 0)
        {
            const int pageSize = 10;

            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
                return NotFound();

            var reviews = await _reviewService.GetTourReviewsAsync(tourId, pageIndex, pageSize);
            ViewBag.TourId = tourId;
            ViewBag.TourName = tour.Name;
            ViewBag.PageIndex = pageIndex;

            return View(reviews);
        }

        // GET: /api/review/can-review/5 - API check xem có thể review
        [HttpGet]
        [Route("api/review/can-review/{orderId}")]
        public async Task<IActionResult> CanReview(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _reviewService.CanReviewAsync(orderId, userId);
            return Json(result);
        }
    }
}

