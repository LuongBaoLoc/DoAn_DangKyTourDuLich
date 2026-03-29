using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Services; // Thêm thư mục chứa EmailService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService; // KHAI BÁO EMAIL SERVICE

        // CONSTRUCTOR: TIÊM EMAIL SERVICE VÀO
        public TourController(ApplicationDbContext context, UserManager<User> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /Tour (Danh sách tour + Tìm kiếm)
        public async Task<IActionResult> Index(TourSearchViewModel searchModel)
        {
            var query = _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive);

            if (!string.IsNullOrEmpty(searchModel.Keyword))
            {
                query = query.Where(t => t.Name.Contains(searchModel.Keyword)
                    || t.ShortDescription.Contains(searchModel.Keyword)
                    || t.Destination.Contains(searchModel.Keyword));
            }

            if (searchModel.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == searchModel.CategoryId.Value);
            }

            if (searchModel.MinPrice.HasValue)
            {
                query = query.Where(t => t.Price >= searchModel.MinPrice.Value);
            }
            if (searchModel.MaxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= searchModel.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchModel.Destination))
            {
                query = query.Where(t => t.Destination.Contains(searchModel.Destination));
            }

            query = searchModel.SortBy switch
            {
                "price_asc" => query.OrderBy(t => t.Price),
                "price_desc" => query.OrderByDescending(t => t.Price),
                "name" => query.OrderBy(t => t.Name),
                "newest" => query.OrderByDescending(t => t.CreatedAt),
                _ => query.OrderByDescending(t => t.IsFeatured).ThenByDescending(t => t.CreatedAt)
            };

            searchModel.TotalItems = await query.CountAsync();
            searchModel.TotalPages = (int)Math.Ceiling((double)searchModel.TotalItems / searchModel.PageSize);

            searchModel.Tours = await query
                .Skip((searchModel.Page - 1) * searchModel.PageSize)
                .Take(searchModel.PageSize)
                .ToListAsync();

            searchModel.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return View(searchModel);
        }

        // GET: /Tour/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (tour == null) return NotFound();

            ViewBag.RelatedTours = await _context.Tours
                .Where(t => t.CategoryId == tour.CategoryId && t.Id != tour.Id && t.IsActive)
                .Take(4)
                .ToListAsync();

            return View(tour);
        }

        // GET: /Tour/Book/5 (Trang nhập thông tin đặt tour)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (tour == null) return NotFound();
            if (tour.AvailableSlots <= 0)
            {
                TempData["Error"] = "Tour này đã hết chỗ.";
                return RedirectToAction("Details", new { id });
            }

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
                TotalAmount = tour.DisplayPrice
            };

            return View(model);
        }

        // POST: /Tour/Book (Xử lý đặt tour + GỬI MAIL)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CheckoutViewModel model)
        {
            var tour = await _context.Tours.FindAsync(model.TourId);
            if (tour == null) return NotFound();

            if (model.Quantity > tour.AvailableSlots)
            {
                ModelState.AddModelError("Quantity", $"Chỉ còn {tour.AvailableSlots} chỗ trống.");
            }

            if (!ModelState.IsValid)
            {
                model.Tour = tour;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var orderCode = "TDL" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);

            var order = new Order
            {
                OrderCode = orderCode,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress,
                Note = model.Note,
                PaymentMethod = model.PaymentMethod,
                TotalAmount = tour.DisplayPrice * model.Quantity,
                UserId = user?.Id,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.Now
            };

            order.OrderDetails.Add(new OrderDetail
            {
                TourId = tour.Id,
                Quantity = model.Quantity,
                UnitPrice = tour.DisplayPrice,
                SubTotal = tour.DisplayPrice * model.Quantity
            });

            tour.CurrentParticipants += model.Quantity;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // --- GỬI THÔNG BÁO CHO ADMIN ---
            try
            {
                // Gửi email thông báo cho Admin có đơn mới
                await _emailService.SendAdminNotificationEmailAsync(
                    order.CustomerName,
                    order.CustomerPhone,
                    tour.Name,
                    order.OrderCode,
                    order.TotalAmount
                );
            }
            catch (Exception ex)
            {
                // Nếu mail lỗi vẫn cho đặt tour thành công, chỉ log lỗi lại
                Console.WriteLine("Lỗi gửi mail tới admin: " + ex.Message);
            }

            TempData["Success"] = $"Đặt tour thành công! Mã đơn hàng: {orderCode}";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        // GET: /Tour/OrderConfirmation/5
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();
            return View(order);
        }

        // GET: /Tour/MyOrders
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}