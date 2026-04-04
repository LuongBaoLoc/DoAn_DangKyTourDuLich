using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentYear = DateTime.Now.Year;

            ViewBag.TotalTours = await _context.Tours.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.PendingOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .CountAsync();

            ViewBag.TotalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Completed)
                .Select(o => (decimal?)o.TotalAmount)
                .SumAsync() ?? 0m;

            var yearlyRevenueRaw = await _context.Orders
                .Where(o => (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Completed) && o.OrderDate.Year == currentYear)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            var monthlyRevenue = Enumerable.Range(1, 12)
                .Select(month => new MonthlyRevenueViewModel
                {
                    Month = month,
                    Label = $"Tháng {month}",
                    Revenue = yearlyRevenueRaw.FirstOrDefault(x => x.Month == month)?.Revenue ?? 0m
                })
                .ToList();

            ViewBag.CurrentYear = currentYear;
            ViewBag.MonthlyRevenue = monthlyRevenue;

            var recentOrders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            var topTours = await _context.OrderDetails
                .Include(od => od.Tour)
                .Where(od => od.Order != null && (od.Order.Status == OrderStatus.Confirmed || od.Order.Status == OrderStatus.Completed))
                .GroupBy(od => new { od.TourId, TourName = od.Tour!.Name, od.Tour.ImageUrl })
                .Select(g => new TopTourViewModel
                {
                    TourId = g.Key.TourId,
                    TourName = g.Key.TourName,
                    ImageUrl = g.Key.ImageUrl,
                    TotalBookings = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.SubTotal)
                })
                .OrderByDescending(x => x.TotalBookings)
                .Take(5)
                .ToListAsync();
            
            ViewBag.TopTours = topTours;

            return View(recentOrders);
        }

        public class MonthlyRevenueViewModel
        {
            public int Month { get; set; }
            public string Label { get; set; } = string.Empty;
            public decimal Revenue { get; set; }
        }

        public class TopTourViewModel
        {
            public int TourId { get; set; }
            public string TourName { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
            public int TotalBookings { get; set; }
            public decimal TotalRevenue { get; set; }
        }
    }
}
