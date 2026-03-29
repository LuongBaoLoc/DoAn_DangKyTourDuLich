using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;

using DoAn_DangKyTourDuLich.Services;

namespace DoAn_DangKyTourDuLich.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public OrderController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(OrderStatus? status)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.User)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
                ViewBag.CurrentStatus = status;
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Admin/Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Nếu hủy đơn thì hoàn lại số chỗ
            if (status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var tour = await _context.Tours.FindAsync(detail.TourId);
                    if (tour != null)
                    {
                        tour.CurrentParticipants -= detail.Quantity;
                    }
                }
            }

            order.Status = status;
            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // --- GỬI EMAIL TƯƠNG ỨNG VỚI TRẠNG THÁI ---
            try
            {
                if (status == OrderStatus.Confirmed)
                {
                    // Gửi email xác nhận đơn với QR code
                    var firstTour = order.OrderDetails.FirstOrDefault()?.Tour?.Name ?? "Tour du lịch";
                    await _emailService.SendBookingEmailAsync(
                        order.CustomerEmail,
                        order.CustomerName,
                        firstTour,
                        order.OrderCode,
                        order.TotalAmount,
                        order.OrderDate
                    );
                    TempData["Success"] = "Cập nhật trạng thái thành 'Đã xác nhận' và gửi email xác nhận cho khách hàng!";
                }
                else if (status == OrderStatus.Completed)
                {
                    TempData["Success"] = "Cập nhật trạng thái thành 'Hoàn thành'!";
                }
                else if (status == OrderStatus.Cancelled)
                {
                    // Gửi email hoàn tiền
                    var firstTour = order.OrderDetails.FirstOrDefault()?.Tour?.Name ?? "Tour du lịch";
                    await _emailService.SendRefundEmailAsync(
                        order.CustomerEmail,
                        order.CustomerName,
                        firstTour,
                        order.OrderCode,
                        order.TotalAmount
                    );
                    TempData["Success"] = "Cập nhật trạng thái thành 'Đã hủy' và gửi email hoàn tiền cho khách hàng!";
                }
                else
                {
                    TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
                TempData["Warning"] = $"Cập nhật trạng thái thành công nhưng gặp lỗi khi gửi email: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/Order/SendEmailToCustomer
        // (KHÔNG CÒN DÙNG - Email sẽ tự động gửi khi admin cập nhật trạng thái)
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> SendEmailToCustomer(int id)
        // {
        //     var order = await _context.Orders
        //         .Include(o => o.OrderDetails)
        //             .ThenInclude(od => od.Tour)
        //         .FirstOrDefaultAsync(o => o.Id == id);

        //     if (order == null) return NotFound();

        //     var tourName = order.OrderDetails.FirstOrDefault()?.Tour?.Name ?? "Tour du lịch";

        //     try
        //     {
        //         await _emailService.SendBookingEmailAsync(
        //             order.CustomerEmail, 
        //             order.CustomerName, 
        //             tourName, 
        //             order.OrderCode, 
        //             order.TotalAmount,
        //             order.OrderDate
        //         );
        //         TempData["Success"] = "Đã gửi email xác nhận cho khách hàng thành công!";
        //     }
        //     catch (Exception ex)
        //     {
        //         TempData["Error"] = "Lỗi khi gửi mail: " + ex.Message;
        //     }

        //     return RedirectToAction(nameof(Details), new { id });
        // }
    }
}
