using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        public async Task<IActionResult> Index(OrderStatus? status, string? type)
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

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "private")
                {
                    query = query.Where(o => o.Note != null && o.Note.ToUpper().Contains("ĐOÀN RIÊNG"));
                }
                else if (type.ToLower() == "group")
                {
                    query = query.Where(o => o.Note == null || !o.Note.ToUpper().Contains("ĐOÀN RIÊNG"));
                }
                ViewBag.CurrentType = type;
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

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

            try
            {
                if (status == OrderStatus.Confirmed)
                {
                    await SendConfirmedOrderEmailAsync(order);
                    TempData["Success"] = "Đã xác nhận chuyến tour và gửi email đầy đủ thông tin cho khách hàng.";
                }
                else if (status == OrderStatus.Completed)
                {
                    TempData["Success"] = "Đã cập nhật trạng thái đơn hàng thành hoàn thành.";
                }
                else if (status == OrderStatus.Cancelled)
                {
                    var firstTour = order.OrderDetails.FirstOrDefault()?.Tour?.Name ?? "Tour du lịch";
                    await _emailService.SendRefundEmailAsync(
                        order.CustomerEmail,
                        order.CustomerName,
                        firstTour,
                        order.OrderCode,
                        order.TotalAmount);

                    TempData["Success"] = "Đã hủy đơn hàng và gửi email thông báo cho khách hàng.";
                }
                else
                {
                    TempData["Success"] = "Đã cập nhật trạng thái đơn hàng thành công.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi gui mail: " + ex.Message);
                TempData["Warning"] = $"Cập nhật trạng thái thành công nhưng gặp lỗi khi gửi email: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task SendConfirmedOrderEmailAsync(Order order)
        {
            var firstDetail = order.OrderDetails.FirstOrDefault();
            var tour = firstDetail?.Tour;

            int adultQty = firstDetail?.Quantity ?? 1;
            int childQty = 0;
            decimal adultPrice = firstDetail?.UnitPrice ?? order.TotalAmount;
            decimal childPrice = adultPrice / 2;
            DateTime? selectedDepartureDateTime = null;
            string groupTypeDisplay = "Tour ghép";

            ParseOrderNote(order.Note, ref adultQty, ref childQty, ref selectedDepartureDateTime, ref groupTypeDisplay);

            string paymentDisplay = order.PaymentMethod switch
            {
                PaymentMethod.CashOnDelivery => "Thanh toán khi nhận tour",
                PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
                PaymentMethod.OnlinePayment => "Thanh toán online",
                _ => "N/A"
            };

            await _emailService.SendBookingEmailAsync(new BookingEmailInfo
            {
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress,
                TourName = tour?.Name ?? "Tour du lịch",
                TourDestination = tour?.Destination,
                TourDepartureLocation = tour?.DepartureLocation,
                TourTransportation = tour?.Transportation,
                TourDuration = tour?.Duration ?? 1,
                TourDepartureDate = tour?.DepartureDate,
                SelectedDepartureDateTime = selectedDepartureDateTime,
                GroupTypeDisplay = groupTypeDisplay,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                ConfirmedAt = order.UpdatedAt,
                AdultQuantity = adultQty,
                ChildQuantity = childQty,
                AdultPrice = adultPrice,
                ChildPrice = childPrice,
                PaymentMethodDisplay = paymentDisplay,
                Note = order.Note
            });
        }

        private static void ParseOrderNote(
            string? note,
            ref int adultQty,
            ref int childQty,
            ref DateTime? selectedDepartureDateTime,
            ref string groupTypeDisplay)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return;
            }

            if (note.Contains("ĐOÀN RIÊNG", StringComparison.OrdinalIgnoreCase))
            {
                groupTypeDisplay = "Đoàn riêng";
            }

            var quantityMatch = Regex.Match(note, @"\[SL:\s*(\d+)\s*L(?:ớ|á»›)n,\s*(\d+)\s*Tr(?:ẻ|áº»)");
            if (quantityMatch.Success)
            {
                adultQty = int.Parse(quantityMatch.Groups[1].Value);
                childQty = int.Parse(quantityMatch.Groups[2].Value);
            }

            var dateMatch = Regex.Match(note, @"Ng(?:à|Ã )y:\s*([0-9]{4}-[0-9]{2}-[0-9]{2})(?:\s+([0-9]{2}:[0-9]{2}))?");
            if (dateMatch.Success)
            {
                var rawDate = dateMatch.Groups[1].Value;
                var rawTime = dateMatch.Groups[2].Success ? dateMatch.Groups[2].Value : "00:00";
                if (DateTime.TryParse($"{rawDate} {rawTime}", out var parsed))
                {
                    selectedDepartureDateTime = parsed;
                }
            }
        }
    }
}
