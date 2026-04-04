using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        public async Task<IActionResult> Create(Tour tour, IFormFile? mainImage, List<IFormFile>? galleryImages)
        {
            if (ModelState.IsValid)
            {
                // Save main image
                if (mainImage != null && mainImage.Length > 0)
                {
                    tour.ImageUrl = await SaveImage(mainImage);
                }

                // Save gallery images
                var savedGalleryImages = await SaveImages(galleryImages);
                if (savedGalleryImages.Count > 0)
                {
                    tour.ImageUrlsData = JsonSerializer.Serialize(savedGalleryImages);
                }

                tour.CreatedAt = DateTime.Now;
                tour.TourCode = await GenerateUniqueTourCodeAsync();
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
        public async Task<IActionResult> Edit(int id, Tour tour, IFormFile? mainImage, List<IFormFile>? galleryImages)
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

                // Handle main image
                if (mainImage != null && mainImage.Length > 0)
                {
                    // Delete old main image if exists
                    if (!string.IsNullOrEmpty(existingTour.ImageUrl))
                    {
                        DeleteImage(existingTour.ImageUrl);
                    }
                    tour.ImageUrl = await SaveImage(mainImage);
                }
                else
                {
                    tour.ImageUrl = existingTour.ImageUrl;
                }

                // Handle gallery images
                if (galleryImages != null && galleryImages.Count > 0)
                {
                    // Delete old gallery images
                    var oldGalleryImages = existingTour.GalleryImages ?? new List<string>();
                    foreach (var imageUrl in oldGalleryImages)
                    {
                        if (imageUrl != tour.ImageUrl) // Don't delete main image
                        {
                            DeleteImage(imageUrl);
                        }
                    }

                    var savedGalleryImages = await SaveImages(galleryImages);
                    tour.ImageUrlsData = savedGalleryImages.Count > 0 ? JsonSerializer.Serialize(savedGalleryImages) : null;
                }
                else
                {
                    tour.ImageUrlsData = existingTour.ImageUrlsData;
                }

                tour.TourCode = string.IsNullOrEmpty(existingTour.TourCode) ? $"T{existingTour.Id:D3}" : existingTour.TourCode; // Keep original tour code or fill fallback
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

        private async Task<string> GenerateUniqueTourCodeAsync()
        {
            var prefix = "T" + DateTime.Now.ToString("yyMM");
            while (true)
            {
                var randomStr = new Random().Next(1000, 9999).ToString();
                var newCode = prefix + randomStr;
                if (!await _context.Tours.AnyAsync(t => t.TourCode == newCode))
                {
                    return newCode;
                }
            }
        }

        public async Task<IActionResult> Schedules(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.TourSchedules)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchedule(int tourId, DateTime departureDate, int maxParticipants, decimal price)
        {
            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null) return NotFound();

            // Check duplicate
            var isAny = await _context.Set<TourSchedule>()
                .AnyAsync(s => s.TourId == tourId && s.DepartureDate.Date == departureDate.Date);

            if (isAny)
            {
                TempData["Error"] = "Lịch khởi hành vào ngày này đã tồn tại.";
                return RedirectToAction(nameof(Schedules), new { id = tourId });
            }

            var schedule = new TourSchedule
            {
                TourId = tourId,
                DepartureDate = departureDate,
                MaxParticipants = maxParticipants,
                Price = price, // Price = 0 implies it uses the original Tour display price (per your model setup)
                IsActive = true
            };

            _context.Set<TourSchedule>().Add(schedule);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm lịch trình thành công!";
            return RedirectToAction(nameof(Schedules), new { id = tourId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSchedule(int id, int tourId)
        {
            var schedule = await _context.Set<TourSchedule>().FindAsync(id);
            if (schedule == null) return NotFound();

            _context.Set<TourSchedule>().Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa lịch trình thành công.";
            return RedirectToAction(nameof(Schedules), new { id = tourId });
        }
    }
}
