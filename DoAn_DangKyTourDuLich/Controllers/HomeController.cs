using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tour nổi bật
            ViewBag.FeaturedTours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive && t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(6)
                .ToListAsync();

            // Lấy danh mục kèm theo danh sách Tours để đếm chính xác số lượng
            ViewBag.Categories = await _context.Categories
                .Include(c => c.Tours)
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // Lấy tour mới nhất
            ViewBag.LatestTours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
