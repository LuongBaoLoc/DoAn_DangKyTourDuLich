using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Repositories.Interfaces
{
    public interface ITourRepository
    {
        Task<Tour?> GetByIdAsync(int id);
        Task<Tour?> GetByIdWithCategoryAsync(int id);
        Task<Tour?> GetByIdWithSchedulesAsync(int id);
        Task<Tour?> GetByIdFullAsync(int id);
        Task<List<Tour>> GetActiveToursAsync();
        Task<List<Tour>> GetActiveToursWithCategoryAsync();
        Task<List<Tour>> GetFeaturedToursAsync(int count = 6);
        Task<List<Tour>> GetLatestToursAsync(int count = 6);
        Task<List<Tour>> SearchToursAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, int? duration, string? transportation);
        Task AddAsync(Tour tour);
        void Update(Tour tour);
        void Remove(Tour tour);
        Task<bool> ExistsAsync(int id);
        Task<bool> TourCodeExistsAsync(string tourCode);
    }
}
