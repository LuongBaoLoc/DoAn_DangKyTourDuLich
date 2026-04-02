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

            return View(recentOrders);
        }

        public class MonthlyRevenueViewModel
        {
            public int Month { get; set; }
            public string Label { get; set; } = string.Empty;
            public decimal Revenue { get; set; }
        }
    }
}
