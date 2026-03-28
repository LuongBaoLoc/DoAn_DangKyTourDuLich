using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tour tour, List<IFormFile>? imageFiles)
        {
            if (ModelState.IsValid)
            {
                var savedImages = await SaveImages(imageFiles);
                if (savedImages.Count > 0)
                {
                    tour.SetGalleryImages(savedImages);
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

        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(),
                "Id", "Name", tour.CategoryId);
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tour tour, List<IFormFile>? imageFiles)
        {
            if (id != tour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingTour = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (existingTour == null)
                {
                    return NotFound();
                }

                var savedImages = await SaveImages(imageFiles);
                if (savedImages.Count > 0)
                {
                    foreach (var imageUrl in existingTour.GalleryImages)
                    {
                        DeleteImage(imageUrl);
                    }

                    tour.SetGalleryImages(savedImages);
                }
                else
                {
                    tour.ImageUrl = existingTour.ImageUrl;
                    tour.ImageUrlsData = existingTour.ImageUrlsData;
                }

                tour.CreatedAt = existingTour.CreatedAt;
                tour.UpdatedAt = DateTime.Now;

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

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            foreach (var imageUrl in tour.GalleryImages)
            {
                DeleteImage(imageUrl);
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa tour thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<string>> SaveImages(List<IFormFile>? files)
        {
            var savedImages = new List<string>();

            if (files == null || files.Count == 0)
            {
                return savedImages;
            }

            foreach (var file in files.Where(file => file.Length > 0))
            {
                savedImages.Add(await SaveImage(file));
            }

            return savedImages;
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "images", "tours");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/tours/" + fileName;
        }

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
