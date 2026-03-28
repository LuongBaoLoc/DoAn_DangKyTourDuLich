using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TourController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Tour
        public async Task<IActionResult> Index(string? keyword, int? categoryId)
        {
            var query = _context.Tours.Include(t => t.Category).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Name.Contains(keyword) || t.Destination.Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
                ViewBag.CategoryId = categoryId;
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name", categoryId);

            var tours = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return View(tours);
        }

        // GET: Admin/Tour/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name");
            return View();
        }

        // POST: Admin/Tour/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tour tour, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    tour.ImageUrl = await SaveImage(imageFile);
                }

                tour.CreatedAt = DateTime.Now;
                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm tour thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name", tour.CategoryId);
            return View(tour);
        }

        // GET: Admin/Tour/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name", tour.CategoryId);
            return View(tour);
        }

        // POST: Admin/Tour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tour tour, IFormFile? imageFile)
        {
            if (id != tour.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingTour = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (existingTour == null) return NotFound();

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingTour.ImageUrl))
                    {
                        DeleteImage(existingTour.ImageUrl);
                    }
                    tour.ImageUrl = await SaveImage(imageFile);
                }
                else
                {
                    tour.ImageUrl = existingTour.ImageUrl;
                }

                tour.UpdatedAt = DateTime.Now;
                tour.CreatedAt = existingTour.CreatedAt;
                _context.Update(tour);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật tour thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name", tour.CategoryId);
            return View(tour);
        }

        // GET: Admin/Tour/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        // POST: Admin/Tour/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            // Xóa ảnh
            if (!string.IsNullOrEmpty(tour.ImageUrl))
            {
                DeleteImage(tour.ImageUrl);
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa tour thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Helper: lưu ảnh
        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "images", "tours");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/tours/" + fileName;
        }

        // Helper: xóa ảnh
        private void DeleteImage(string imageUrl)
        {
            var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
