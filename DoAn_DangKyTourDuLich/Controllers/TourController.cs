using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using DoAn_DangKyTourDuLich.Services;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Controllers
{
    /// <summary>
    /// Controller đã được refactor — business logic đã chuyển sang ITourService.
    /// Controller chỉ đảm nhận: nhận request → gọi service → trả view.
    /// </summary>
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly ILogger<TourController> _logger;

        public TourController(
            ITourService tourService,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            EmailService emailService,
            ILogger<TourController> logger)
        {
            _tourService = tourService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(TourSearchViewModel searchModel)
        {
            var result = await _tourService.SearchToursAsync(searchModel);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string? term, int limit = 8)
        {
            var suggestions = await _tourService.GetSuggestionsAsync(term, limit);
            return Json(suggestions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _tourService.GetTourDetailsAsync(id);
            if (model == null) return NotFound();

            var schedulesJson = model.Tour.TourSchedules
                .Where(ts => ts.IsActive)
                .Select(ts => new
                {
                    date = ts.DepartureDate.ToString("yyyy-MM-dd"),
                    price = ts.Price > 0 ? ts.Price : model.Tour.DisplayPrice
                }).ToList();
            ViewBag.SchedulesJson = System.Text.Json.JsonSerializer.Serialize(schedulesJson);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book(int id, string? date, string? time)
        {
            var tour = await _unitOfWork.Tours.GetByIdWithSchedulesAsync(id);
            if (tour == null || !tour.IsActive) return NotFound();

            if (tour.AvailableSlots <= 0)
            {
                TempData["Error"] = "Tour này hiện đã hết chỗ ghép lẻ.";
                return RedirectToAction("Details", new { id });
            }

            var selectedDateTime = string.IsNullOrWhiteSpace(date)
                ? string.Empty
                : string.IsNullOrWhiteSpace(time) ? date : $"{date} {time}";

            var user = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                TourId = tour.Id,
                Tour = tour,
                CustomerName = user?.FullName ?? "",
                CustomerEmail = user?.Email ?? "",
                CustomerPhone = user?.PhoneNumber ?? "",
                CustomerAddress = user?.Address ?? "",
                Quantity = 1,
                AdultQuantity = 1,
                SelectedDate = selectedDateTime,
                TotalAmount = tour.DisplayPrice
            };

            bool isCustomDate = true;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
            {
                isCustomDate = !tour.TourSchedules.Any(ts => ts.DepartureDate.Date == parsedDate.Date && ts.IsActive);
            }
            ViewBag.IsCustomDate = isCustomDate;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CheckoutViewModel model)
        {
            var tour = await _unitOfWork.Tours.GetByIdWithSchedulesAsync(model.TourId);
            if (tour == null) return NotFound();

            bool isPrivateGroup = (model.AdultQuantity + model.ChildQuantity) >= 10;

            bool isCustomDate = true;
            string dateOnly = model.SelectedDate?.Split(' ')[0] ?? "";
            if (!string.IsNullOrEmpty(dateOnly) && DateTime.TryParse(dateOnly, out var parsedDate))
            {
                isCustomDate = !tour.TourSchedules.Any(ts => ts.DepartureDate.Date == parsedDate.Date && ts.IsActive);
            }

            if (isCustomDate && !isPrivateGroup)
                ModelState.AddModelError("", "Ngày khởi hành tự chọn là Tour đoàn riêng, yêu cầu tối thiểu 10 người tham gia.");

            if (!isPrivateGroup && model.Quantity > tour.AvailableSlots)
                ModelState.AddModelError("Quantity", $"Tour ghép hiện chỉ còn {tour.AvailableSlots} chỗ.");

            if (!ModelState.IsValid)
            {
                model.Tour = tour;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            string orderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            decimal childPrice = tour.DisplayPrice / 2;
            decimal totalAmount = (model.AdultQuantity * tour.DisplayPrice) + (model.ChildQuantity * childPrice);

            string groupType = isPrivateGroup ? " [ĐOÀN RIÊNG]" : " [TOUR GHÉP]";
            string detailNote = $"{groupType}\n[SL: {model.AdultQuantity} Lớn, {model.ChildQuantity} Trẻ | Ngày: {model.SelectedDate}]";

            var order = new Order
            {
                OrderCode = orderCode,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress,
                Note = detailNote + (string.IsNullOrEmpty(model.Note) ? "" : "\n" + model.Note),
                PaymentMethod = model.PaymentMethod,
                TotalAmount = totalAmount,
                UserId = user?.Id,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            order.OrderDetails.Add(new OrderDetail
            {
                TourId = tour.Id,
                Quantity = model.Quantity,
                UnitPrice = tour.DisplayPrice,
                SubTotal = totalAmount
            });

            if (!isPrivateGroup)
                tour.CurrentParticipants += model.Quantity;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Đơn hàng mới: {OrderCode} - Tour: {TourName} - Khách: {CustomerName}",
                orderCode, tour.Name, model.CustomerName);

            if (order.PaymentMethod == PaymentMethod.OnlinePayment)
                return RedirectToAction("CreatePayment", "VnPay", new { orderId = order.Id });

            if (order.PaymentMethod != PaymentMethod.CashOnDelivery)
                return RedirectToAction("Payment", new { orderId = order.Id });

            TempData["Success"] = isPrivateGroup ? "Yêu cầu đoàn riêng đã gửi!" : "Đặt tour thành công!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> Payment(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Pending;
                order.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("OrderConfirmation", new { orderId });
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _unitOfWork.Orders.GetByUserIdAsync(user!.Id);
            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> DownloadInvoice([FromServices] PdfInvoiceService pdfService, int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);

            if (order == null || order.UserId != user!.Id) return NotFound();

            var pdfBytes = pdfService.GenerateInvoice(order);
            return File(pdfBytes, "application/pdf", $"Invoice_{order.OrderCode}.pdf");
        }
    }
}
