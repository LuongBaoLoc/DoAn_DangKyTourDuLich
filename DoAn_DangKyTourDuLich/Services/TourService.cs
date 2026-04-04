using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace DoAn_DangKyTourDuLich.Services
{
    /// <summary>
    /// Service chứa toàn bộ business logic liên quan đến Tour.
    /// Được tách ra từ TourController để tuân thủ Single Responsibility Principle.
    /// </summary>
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TourService> _logger;

        public TourService(IUnitOfWork unitOfWork, ApplicationDbContext context, ILogger<TourService> logger)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _logger = logger;
        }

        public async Task<TourSearchViewModel> SearchToursAsync(TourSearchViewModel searchModel)
        {
            _logger.LogInformation("Tìm kiếm tour với keyword: {Keyword}, CategoryId: {CategoryId}", 
                searchModel.Keyword, searchModel.CategoryId);

            var tours = await _unitOfWork.Tours.SearchToursAsync(
                searchModel.CategoryId,
                searchModel.MinPrice,
                searchModel.MaxPrice,
                searchModel.Duration,
                searchModel.Transportation);

            // Lọc theo keyword (client-side vì cần normalize tiếng Việt)
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

            // Lọc theo điểm đến
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

            // Phân trang
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

            _logger.LogInformation("Tìm thấy {Count} tour phù hợp", searchModel.TotalItems);
            return searchModel;
        }

        public async Task<TourDetailsViewModel?> GetTourDetailsAsync(int id)
        {
            var currentTour = await _unitOfWork.Tours.GetByIdWithSchedulesAsync(id);
            if (currentTour == null)
            {
                _logger.LogWarning("Không tìm thấy tour với Id: {TourId}", id);
                return null;
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

            return model;
        }

        public async Task<List<object>> GetSuggestionsAsync(string? term, int limit = 8)
        {
            string normalizedTerm = NormalizeVietnamese(term);
            if (string.IsNullOrWhiteSpace(normalizedTerm))
                return new List<object>();

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
                .GroupBy(s => NormalizeVietnamese(s!.Text))
                .Select(g => g
                    .OrderByDescending(x => x!.Priority)
                    .ThenBy(x => x!.Text.Length)
                    .First()!)
                .OrderByDescending(s => s.Priority)
                .ThenBy(s => s.Text.Length)
                .Take(limit)
                .Select(s => (object)new { text = s.Text, type = s.Type })
                .ToList();

            return suggestions;
        }

        /// <summary>
        /// Chuẩn hóa chuỗi tiếng Việt — bỏ dấu, lowercase, chỉ giữ chữ và số.
        /// Sử dụng Unicode Normalization Form D để tách dấu khỏi ký tự gốc.
        /// </summary>
        public string NormalizeVietnamese(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (char c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (char.IsLetterOrDigit(c) || c == 'đ' || c == 'Đ')
                        builder.Append(c);
                }
            }

            return builder.ToString()
                .Replace('đ', 'd')
                .Replace('Đ', 'D')
                .Normalize(NormalizationForm.FormC);
        }

        #region Private Helpers

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
            if (sameCategory) reasons.Add($"Cùng danh mục {(currentTour.Category?.Name ?? "tour")}");
            if (sameDestination) reasons.Add($"Cùng điểm đến {currentTour.Destination}");
            if (similarPrice) reasons.Add("Mức giá tương đồng");

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

        private static double CalculateSimilarity(Tour t1, Tour t2)
        {
            double score = 0;

            if (t1.CategoryId == t2.CategoryId) score += 0.6;

            if (!string.IsNullOrEmpty(t1.Destination) && !string.IsNullOrEmpty(t2.Destination))
            {
                if (t1.Destination.Trim().Equals(t2.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
                    score += 0.3;
            }

            if (t1.DisplayPrice > 0)
            {
                double priceDiff = (double)Math.Abs(t1.DisplayPrice - t2.DisplayPrice) / (double)t1.DisplayPrice;
                if (priceDiff <= 0.2) score += 0.1;
            }

            return score;
        }

        private SuggestionItem? CreateSuggestion(string? value, string type, string normalizedTerm)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            string normalizedValue = NormalizeVietnamese(value);
            if (string.IsNullOrWhiteSpace(normalizedValue)) return null;

            int priority = normalizedValue.StartsWith(normalizedTerm, StringComparison.Ordinal) ? 2 :
                normalizedValue.Contains(normalizedTerm, StringComparison.Ordinal) ? 1 : 0;

            return priority == 0 ? null : new SuggestionItem { Text = value.Trim(), Type = type, Priority = priority };
        }

        private sealed class SuggestionItem
        {
            public string Text { get; init; } = string.Empty;
            public string Type { get; init; } = string.Empty;
            public int Priority { get; init; }
        }

        #endregion
    }
}
