using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Review/AddReview/5
        public async Task<IActionResult> AddReview(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Nếu đã có review từ trước thì hiển thị review cũ
            var existingReview = order.Reviews?.FirstOrDefault();
            if (existingReview != null)
            {
                ViewBag.HasReview = true;
                ViewBag.ReviewRating = existingReview.Rating;
                ViewBag.ReviewComment = existingReview.Comment;
            }

            return View(order);
        }

        // POST: /Review/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int orderId, int rating, string? comment)
        {
            var order = await _context.Orders
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("", "Vui lòng chọn xếp hạng từ 1 đến 5 sao");
                return View(order);
            }

            // Kiểm tra xem đã có review chưa - nếu có thì cập nhật, nếu chưa thì tạo mới
            var existingReview = order.Reviews?.FirstOrDefault();
            if (existingReview != null)
            {
                // Cập nhật review cũ
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.Now;
                _context.Reviews.Update(existingReview);
                TempData["Success"] = "Cập nhật đánh giá thành công! Cảm ơn bạn! 😊";
            }
            else
            {
                // Tạo review mới
                var review = new Review
                {
                    OrderId = orderId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                TempData["Success"] = "Gửi đánh giá thành công! Cảm ơn bạn! 😊";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { id = orderId });
        }

        // GET: /Review/Success/5
        public async Task<IActionResult> Success(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
