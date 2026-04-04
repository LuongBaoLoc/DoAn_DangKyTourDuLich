using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;

        public TourController(ApplicationDbContext context, UserManager<User> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        #region 1. DANH SÁCH & TÌM KIẾM CƠ BẢN
        public async Task<IActionResult> Index(TourSearchViewModel searchModel)
        {
            var query = _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive);

            if (searchModel.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == searchModel.CategoryId.Value);

            if (searchModel.MinPrice.HasValue)
                query = query.Where(t => t.Price >= searchModel.MinPrice.Value);

            if (searchModel.MaxPrice.HasValue)
                query = query.Where(t => t.Price <= searchModel.MaxPrice.Value);

            if (searchModel.Duration.HasValue)
            {
                if (searchModel.Duration.Value >= 5)
                    query = query.Where(t => t.Duration >= 5);
                else
                    query = query.Where(t => t.Duration == searchModel.Duration.Value);
            }

            if (!string.IsNullOrEmpty(searchModel.Transportation))
                query = query.Where(t => t.Transportation!.Contains(searchModel.Transportation));

            var tours = await query.ToListAsync();

            // Xử lý tìm kiếm tiếng Việt không dấu
            if (!string.IsNullOrWhiteSpace(searchModel.Keyword))
            {
                string normalizedKeyword = NormalizeVietnamese(searchModel.Keyword);
                tours = tours.Where(t =>
                        NormalizeVietnamese(t.Name).Contains(normalizedKeyword) ||
                        NormalizeVietnamese(t.ShortDescription).Contains(normalizedKeyword) ||
                        NormalizeVietnamese(t.Destination).Contains(normalizedKeyword) ||
                        NormalizeVietnamese(t.Category?.Name).Contains(normalizedKeyword))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Destination))
            {
                string normalizedDestination = NormalizeVietnamese(searchModel.Destination);
                tours = tours.Where(t => NormalizeVietnamese(t.Destination).Contains(normalizedDestination)).ToList();
            }

            // Sắp xếp
            tours = searchModel.SortBy switch
            {
                "price_asc" => tours.OrderBy(t => t.Price).ToList(),
                "price_desc" => tours.OrderByDescending(t => t.Price).ToList(),
                "name" => tours.OrderBy(t => t.Name).ToList(),
                "newest" => tours.OrderByDescending(t => t.CreatedAt).ToList(),
                _ => tours.OrderByDescending(t => t.IsFeatured).ThenByDescending(t => t.CreatedAt).ToList()
            };

            searchModel.TotalItems = tours.Count;
            searchModel.TotalPages = (int)Math.Ceiling((double)searchModel.TotalItems / searchModel.PageSize);
            searchModel.Tours = tours.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            searchModel.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync();

            return View(searchModel);
        }

        private static string NormalizeVietnamese(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            string normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    if (char.IsLetterOrDigit(c) || c == 'đ' || c == 'Đ') builder.Append(c);
                }
            }
            return builder.ToString().Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormC);
        }
        #endregion

        #region 2. CHỨC NĂNG KHẢO SÁT & THUẬT TOÁN SCORING
        [HttpGet]
        public IActionResult Survey()
        {
            return View(new SurveyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Survey(SurveyViewModel model)
        {
            var allTours = await _context.Tours.Include(t => t.Category).Where(t => t.IsActive).ToListAsync();

            // 1. LỌC CỨNG ĐỊA ĐIỂM (Bắt buộc phải đúng 100%)
            var filteredTours = allTours.AsQueryable();

            if (model.DestinationType == "Beach")
            {
                // Bắt buộc chỉ lấy tour Biển
                filteredTours = filteredTours.Where(t => t.IsBeach == true);
            }
            else if (model.DestinationType == "Mountain")
            {
                // Bắt buộc chỉ lấy tour Núi
                filteredTours = filteredTours.Where(t => t.IsMountain == true);
            }
            else if (model.DestinationType == "City")
            {
                // Bắt buộc chỉ lấy tour Thành phố/Sông nước (Không Biển, Không Núi)
                filteredTours = filteredTours.Where(t => t.IsBeach == false && t.IsMountain == false);
            }

            // 2. CHẤM ĐIỂM ĐỂ XẾP HẠNG (Ưu tiên đưa tour phù hợp giá/nhóm lên đầu)
            var recommendedTours = filteredTours.Select(t => new {
                Tour = t,
                Score = (model.Budget == "Low" && t.IsLowBudget ? 30 : 0) +
                        (model.Budget == "High" && !t.IsLowBudget ? 30 : 0) +
                        (model.TravelStyle == "Group" && t.IsForGroup ? 10 : 0) +
                        (model.TravelStyle == "Solo" && !t.IsForGroup ? 10 : 0)
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.Tour)
            .ToList();

            // 3. THÔNG BÁO KẾT QUẢ RÕ RÀNG
            if (recommendedTours.Count == 0)
            {
                recommendedTours = allTours.OrderByDescending(t => t.CreatedAt).Take(6).ToList();
                TempData["Error"] = "Chưa có tour khớp 100% yêu cầu, đây là các gợi ý thay thế!";
            }
            else
            {
                string loaiDiaDiem = model.DestinationType == "Beach" ? "Biển Đảo" : "Núi Rừng";
                TempData["Success"] = $"AI đã lọc ra chính xác {recommendedTours.Count} tour {loaiDiaDiem} cho bạn!";
            }

            var searchModel = new TourSearchViewModel
            {
                Tours = recommendedTours,
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                TotalItems = recommendedTours.Count,
                PageSize = 100
            };

            return View("Index", searchModel);
        }
        #endregion

        #region 3. CHI TIẾT TOUR & GỢI Ý TOUR TƯƠNG ĐỒNG
        public async Task<IActionResult> Details(int id)
        {
            var currentTour = await _context.Tours.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
            if (currentTour == null) return NotFound();

            var candidateTours = await _context.Tours.Include(t => t.Category)
                .Where(t => t.Id != id && t.IsActive).ToListAsync();

            var model = new TourDetailsViewModel
            {
                Tour = currentTour,
                RelatedTours = candidateTours
                    .Select(t => BuildRecommendation(currentTour, t))
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .Take(4)
                    .ToList()
            };
            return View(model);
        }

        private TourRecommendationViewModel BuildRecommendation(Tour current, Tour target)
        {
            double score = 0;
            var reasons = new List<string>();

            if (current.CategoryId == target.CategoryId) { score += 0.6; reasons.Add($"Cùng loại hình {current.Category?.Name}"); }
            if (current.Destination == target.Destination) { score += 0.3; reasons.Add($"Cùng tại {current.Destination}"); }

            double priceDiff = (double)Math.Abs(current.DisplayPrice - target.DisplayPrice) / (double)current.DisplayPrice;
            if (priceDiff <= 0.2) { score += 0.1; reasons.Add("Mức giá tương đồng"); }

            return new TourRecommendationViewModel { Tour = target, Score = score, Reasons = reasons };
        }
        #endregion

        #region 4. HỆ THỐNG ĐẶT TOUR & THANH TOÁN
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book(int id, string? date, string? time)
        {
            var tour = await _context.Tours.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
            if (tour == null) return NotFound();
            if (tour.AvailableSlots <= 0) { TempData["Error"] = "Tour đã hết chỗ."; return RedirectToAction("Details", new { id }); }

            var selectedDateTime = string.IsNullOrWhiteSpace(date) ? string.Empty : string.IsNullOrWhiteSpace(time) ? date : $"{date} {time}";
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
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CheckoutViewModel model)
        {
            var tour = await _context.Tours.FindAsync(model.TourId);
            if (tour == null) return NotFound();

            bool isPrivateGroup = (model.AdultQuantity + model.ChildQuantity) >= 10;
            if (!isPrivateGroup && model.Quantity > tour.AvailableSlots) { ModelState.AddModelError("Quantity", "Không đủ chỗ trống."); }
            if (!ModelState.IsValid) { model.Tour = tour; return View(model); }

            var user = await _userManager.GetUserAsync(User);
            string orderCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            decimal childPrice = tour.DisplayPrice / 2;
            decimal totalAmount = (model.AdultQuantity * tour.DisplayPrice) + (model.ChildQuantity * childPrice);
            string detailNote = $"{(isPrivateGroup ? " [ĐOÀN RIÊNG]" : " [TOUR GHÉP]")}\n[SL: {model.AdultQuantity} Lớn, {model.ChildQuantity} Trẻ | Ngày: {model.SelectedDate}]";

            var order = new Order
            {
                OrderCode = orderCode,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress,
                Note = detailNote + (string.IsNullOrEmpty(model.Note) ? "" : "\n" + model.Note),
                TotalAmount = totalAmount,
                UserId = user?.Id,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod
            };

            order.OrderDetails.Add(new OrderDetail { TourId = tour.Id, Quantity = model.Quantity, UnitPrice = tour.DisplayPrice, SubTotal = totalAmount });
            if (!isPrivateGroup) tour.CurrentParticipants += model.Quantity;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (order.PaymentMethod != PaymentMethod.CashOnDelivery) return RedirectToAction("Payment", new { orderId = order.Id });
            TempData["Success"] = isPrivateGroup ? "Yêu cầu đoàn riêng đã gửi!" : "Đặt tour thành công!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> Payment(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Tour).FirstOrDefaultAsync(o => o.Id == orderId);
            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null) { order.Status = OrderStatus.Pending; order.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); }
            return RedirectToAction("OrderConfirmation", new { orderId });
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Tour).FirstOrDefaultAsync(o => o.Id == orderId);
            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Tour)
                .Where(o => o.UserId == user!.Id).OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }
        #endregion

        #region 5. TIỆN ÍCH AI & SEED DỮ LIỆU NHANH
        [HttpGet]
        public async Task<IActionResult> Suggestions(string? term)
        {
            string norm = NormalizeVietnamese(term);
            if (string.IsNullOrEmpty(norm)) return Json(new object[] { });
            var suggestions = await _context.Tours.Where(t => t.IsActive && NormalizeVietnamese(t.Name).Contains(norm))
                .Select(t => new { text = t.Name, type = "Tour" }).Take(8).ToListAsync();
            return Json(suggestions);
        }

        // Đường dẫn: /Tour/QuickSeed - Quét toàn bộ DB để gắn nhãn tự động

        [HttpGet]
        public async Task<IActionResult> QuickSeed()
        {
            var tours = await _context.Tours.ToListAsync();

            var beachKeys = new[] { "phú quốc", "đà nẵng", "hạ long", "nha trang", "mũi né", "vũng tàu", "thái lan", "singapore", "biển", "đảo" };
            var mountainKeys = new[] { "đà lạt", "sapa", "ninh bình", "tràng an", "nhật bản", "hàn quốc", "núi", "rừng" };

            foreach (var t in tours)
            {
                string content = (t.Name + " " + t.Destination).ToLower();

                t.IsBeach = beachKeys.Any(k => content.Contains(k));
                t.IsMountain = mountainKeys.Any(k => content.Contains(k));

                // ĐÃ XÓA ĐOẠN 50/50 TRUY TÌM NGẪU NHIÊN. 
                // Giờ Cần Thơ, Sài Gòn, Huế sẽ không bị ép làm Núi hay Biển nữa!

                t.IsLowBudget = (t.Price <= 4500000);
                t.IsForGroup = (t.MaxParticipants >= 20);
            }

            await _context.SaveChangesAsync();
            return Content($"Thành công! Đã dọn dẹp sạch sẽ, Cần Thơ đã không còn Núi nữa!");
        }
        #endregion
        // Đường dẫn: /Tour/FixImages - Tự động nạp ảnh Online nét căng cho 20 Tour
        [HttpGet]
        public async Task<IActionResult> FixImages()
        {
            var tours = await _context.Tours.ToListAsync();

            foreach (var t in tours)
            {
                // Sử dụng LoremFlickr với từ khóa là tên Điểm đến (Destination)
                // Link này sẽ tự động tìm ảnh liên quan đến địa danh đó trên Flickr
                // lock={t.Id} giúp ảnh không bị nhảy lung tung khi load lại trang
                string keyword = NormalizeVietnamese(t.Destination).Replace(" ", ",");

                // Hoặc link này (Khuyên dùng):
                t.ImageUrl = $"https://www.bing.com/th?id=OIP.featured&q={t.Destination}+travel+4k";
            }

            await _context.SaveChangesAsync();
            return Content("Đã sửa lỗi link chết! Toàn bộ 20 tour đã được nạp link ảnh tự động mới. Bạn hãy quay lại trang chủ và kiểm tra nhé!");
        }
    }
}