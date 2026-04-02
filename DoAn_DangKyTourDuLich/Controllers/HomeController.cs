using System.Diagnostics;
using System.Globalization;
using System.Text;
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

        public async Task<IActionResult> Index(string? searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string processedTerm = NormalizeVietnamese(searchTerm);
                if (processedTerm.Contains("thanh pho suong mu")) processedTerm = "da lat";
                if (processedTerm.Contains("dao ngoc")) processedTerm = "phu quoc";

                var allTours = await _context.Tours
                    .Include(t => t.Category)
                    .Where(t => t.IsActive)
                    .ToListAsync();

                var searchResults = allTours
                    .Where(t =>
                        NormalizeVietnamese(t.Name).Contains(processedTerm) ||
                        NormalizeVietnamese(t.ShortDescription).Contains(processedTerm) ||
                        NormalizeVietnamese(t.Destination).Contains(processedTerm) ||
                        NormalizeVietnamese(t.Category?.Name).Contains(processedTerm))
                    .ToList();

                ViewBag.SearchTerm = searchTerm;
                return View("SearchResult", searchResults);
            }

            ViewBag.FeaturedTours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive && t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(6)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories
                .Include(c => c.Tours)
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            ViewBag.LatestTours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null) return NotFound();

            var allTours = await _context.Tours.Where(t => t.IsActive && t.Id != id).ToListAsync();

            double maxPrice = allTours.Any() ? (double)allTours.Max(t => t.Price) : 1.0;
            if (maxPrice == 0) maxPrice = 1.0;

            var currentVector = new double[] { (double)tour.CategoryId, (double)tour.Price / maxPrice };

            ViewBag.RecommendedTours = allTours
                .Select(t => new
                {
                    Tour = t,
                    Score = CalculateCosineSimilarity(currentVector, new double[] { (double)t.CategoryId, (double)t.Price / maxPrice })
                })
                .OrderByDescending(x => x.Score)
                .Take(4)
                .Select(x => x.Tour)
                .ToList();

            return View(tour);
        }

        private double CalculateCosineSimilarity(double[] vA, double[] vB)
        {
            double dotProduct = 0, magA = 0, magB = 0;
            for (int i = 0; i < vA.Length; i++)
            {
                dotProduct += vA[i] * vB[i];
                magA += Math.Pow(vA[i], 2);
                magB += Math.Pow(vB[i], 2);
            }
            return (magA == 0 || magB == 0) ? 0 : dotProduct / (Math.Sqrt(magA) * Math.Sqrt(magB));
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
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
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

        public IActionResult Privacy() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
