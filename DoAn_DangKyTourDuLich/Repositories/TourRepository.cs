using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tour?> GetByIdAsync(int id)
        {
            return await _context.Tours.FindAsync(id);
        }

        public async Task<Tour?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour?> GetByIdWithSchedulesAsync(int id)
        {
            return await _context.Tours
                .Include(t => t.Category)
                .Include(t => t.TourSchedules)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour?> GetByIdFullAsync(int id)
        {
            return await _context.Tours
                .Include(t => t.Category)
                .Include(t => t.TourSchedules)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tour>> GetActiveToursAsync()
        {
            return await _context.Tours
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetActiveToursWithCategoryAsync()
        {
            return await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetFeaturedToursAsync(int count = 6)
        {
            return await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive && t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetLatestToursAsync(int count = 6)
        {
            return await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Tour>> SearchToursAsync(
            int? categoryId, decimal? minPrice, decimal? maxPrice,
            int? duration, string? transportation)
        {
            var query = _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.Price <= maxPrice.Value);

            if (duration.HasValue)
            {
                query = duration.Value >= 5
                    ? query.Where(t => t.Duration >= 5)
                    : query.Where(t => t.Duration == duration.Value);
            }

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            return await query.ToListAsync();
        }

        public async Task AddAsync(Tour tour)
        {
            await _context.Tours.AddAsync(tour);
        }

        public void Update(Tour tour)
        {
            _context.Tours.Update(tour);
        }

        public void Remove(Tour tour)
        {
            _context.Tours.Remove(tour);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tours.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TourCodeExistsAsync(string tourCode)
        {
            return await _context.Tours.AnyAsync(t => t.TourCode == tourCode);
        }
    }
}
