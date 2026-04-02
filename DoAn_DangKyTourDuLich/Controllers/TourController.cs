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

        public async Task<IActionResult> Index(TourSearchViewModel searchModel)
        {
            var query = _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive);

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

            if (searchModel.Duration.HasValue)
            {
                if (searchModel.Duration.Value >= 5)
                {
                    query = query.Where(t => t.Duration >= 5);
                }
                else
                {
                    query = query.Where(t => t.Duration == searchModel.Duration.Value);
                }
            }

            if (!string.IsNullOrEmpty(searchModel.Transportation))
            {
                query = query.Where(t => t.Transportation!.Contains(searchModel.Transportation));
            }

            var tours = await query.ToListAsync();

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

            searchModel.Tours = tours
                .Skip((searchModel.Page - 1) * searchModel.PageSize)
                .Take(searchModel.PageSize)
                .ToList();

            searchModel.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return View(searchModel);
        }

        private static string NormalizeVietnamese(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (char.IsLetterOrDigit(c) || c == 'đ' || c == 'Đ')
                    {
                        builder.Append(c);
                    }
                }
            }

            return builder.ToString()
                .Replace('đ', 'd')
                .Replace('Đ', 'D')
                .Normalize(NormalizationForm.FormC);
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string? term, int limit = 8)
        {
            string normalizedTerm = NormalizeVietnamese(term);
            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return Json(Array.Empty<object>());
            }

            limit = Math.Clamp(limit, 1, 12);

            var tours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    t.Name,
                    t.Destination,
                    CategoryName = t.Category != null ? t.Category.Name : null
                })
                .ToListAsync();

            var suggestions = tours
                .SelectMany(t => new[]
                {
                    CreateSuggestion(t.Name, "Tour", normalizedTerm),
                    CreateSuggestion(t.Destination, "Điểm đến", normalizedTerm),
                    CreateSuggestion(t.CategoryName, "Loại hình", normalizedTerm)
                })
                .Where(s => s != null)
                .Cast<SuggestionItem>()
                .GroupBy(s => NormalizeVietnamese(s.Text))
                .Select(g => g
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Text.Length)
                    .First())
                .OrderByDescending(s => s.Priority)
                .ThenBy(s => s.Text.Length)
                .Take(limit)
                .Select(s => new
                {
                    text = s.Text,
                    type = s.Type
                })
                .ToList();

            return Json(suggestions);
        }

        private static SuggestionItem? CreateSuggestion(string? value, string type, string normalizedTerm)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string normalizedValue = NormalizeVietnamese(value);
            if (string.IsNullOrWhiteSpace(normalizedValue))
            {
                return null;
            }

            int priority = normalizedValue.StartsWith(normalizedTerm, StringComparison.Ordinal) ? 2 :
                normalizedValue.Contains(normalizedTerm, StringComparison.Ordinal) ? 1 : 0;

            if (priority == 0)
            {
                return null;
            }

            return new SuggestionItem
            {
                Text = value.Trim(),
                Type = type,
                Priority = priority
            };
        }

        private sealed class SuggestionItem
        {
            public string Text { get; init; } = string.Empty;
            public string Type { get; init; } = string.Empty;
            public int Priority { get; init; }
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentTour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (currentTour == null)
            {
                return NotFound();
            }

            var candidateTours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.Id != id && t.IsActive)
                .ToListAsync();

            var model = new TourDetailsViewModel
            {
                Tour = currentTour,
                RelatedTours = candidateTours
                    .Select(t => BuildRecommendation(currentTour, t))
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Tour.IsFeatured)
                    .ThenBy(x => Math.Abs(x.Tour.DisplayPrice - currentTour.DisplayPrice))
                    .Take(4)
                    .ToList()
            };

            return View(model);
        }

        private TourRecommendationViewModel BuildRecommendation(Tour currentTour, Tour candidateTour)
        {
            bool sameCategory = currentTour.CategoryId == candidateTour.CategoryId;
            bool sameDestination =
                !string.IsNullOrWhiteSpace(currentTour.Destination) &&
                !string.IsNullOrWhiteSpace(candidateTour.Destination) &&
                currentTour.Destination.Trim().Equals(candidateTour.Destination.Trim(), StringComparison.OrdinalIgnoreCase);

            bool similarPrice = false;
            if (currentTour.DisplayPrice > 0)
            {
                double priceDiff = (double)Math.Abs(currentTour.DisplayPrice - candidateTour.DisplayPrice) / (double)currentTour.DisplayPrice;
                similarPrice = priceDiff <= 0.2;
            }

            var reasons = new List<string>();
            if (sameCategory)
            {
                reasons.Add($"Cùng danh mục {(currentTour.Category?.Name ?? "tour")}");
            }
            if (sameDestination)
            {
                reasons.Add($"Cùng điểm đến {currentTour.Destination}");
            }
            if (similarPrice)
            {
                reasons.Add("Mức giá tương đồng");
            }

            return new TourRecommendationViewModel
            {
                Tour = candidateTour,
                Score = CalculateSimilarity(currentTour, candidateTour),
                SameCategory = sameCategory,
                SameDestination = sameDestination,
                SimilarPrice = similarPrice,
                Reasons = reasons
            };
        }

        private double CalculateSimilarity(Tour t1, Tour t2)
        {
            double score = 0;

            if (t1.CategoryId == t2.CategoryId)
            {
                score += 0.6;
            }

            if (!string.IsNullOrEmpty(t1.Destination) && !string.IsNullOrEmpty(t2.Destination))
            {
                if (t1.Destination.Trim().Equals(t2.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    score += 0.3;
                }
            }

            if (t1.DisplayPrice > 0)
            {
                double priceDiff = (double)Math.Abs(t1.DisplayPrice - t2.DisplayPrice) / (double)t1.DisplayPrice;
                if (priceDiff <= 0.2)
                {
                    score += 0.1;
                }
            }

            return score;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book(int id, string? date, string? time)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (tour == null) return NotFound();

            if (tour.AvailableSlots <= 0)
            {
                TempData["Error"] = "Tour này hiện đã hết chỗ ghép lẻ.";
                return RedirectToAction("Details", new { id });
            }

            var selectedDateTime = string.IsNullOrWhiteSpace(date)
                ? string.Empty
                : string.IsNullOrWhiteSpace(time)
                    ? date
                    : $"{date} {time}";

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

            if (!isPrivateGroup && model.Quantity > tour.AvailableSlots)
            {
                ModelState.AddModelError("Quantity", $"Tour ghép hiện chỉ còn {tour.AvailableSlots} chỗ.");
            }

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
                OrderDate = DateTime.Now
            };

            order.OrderDetails.Add(new OrderDetail
            {
                TourId = tour.Id,
                Quantity = model.Quantity,
                UnitPrice = tour.DisplayPrice,
                SubTotal = totalAmount
            });

            if (!isPrivateGroup)
            {
                tour.CurrentParticipants += model.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (order.PaymentMethod != PaymentMethod.CashOnDelivery)
            {
                return RedirectToAction("Payment", new { orderId = order.Id });
            }

            TempData["Success"] = isPrivateGroup ? "Yêu cầu đoàn riêng đã gửi!" : "Đặt tour thành công!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> Payment(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Pending;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OrderConfirmation", new { orderId });
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Tour)
                .Where(o => o.UserId == user!.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}
