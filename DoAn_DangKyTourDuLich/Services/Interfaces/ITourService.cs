using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;

namespace DoAn_DangKyTourDuLich.Services.Interfaces
{
    public interface ITourService
    {
        /// <summary>
        /// Tìm kiếm tour với nhiều bộ lọc và phân trang
        /// </summary>
        Task<TourSearchViewModel> SearchToursAsync(TourSearchViewModel searchModel);

        /// <summary>
        /// Lấy chi tiết tour kèm gợi ý tour liên quan (Content-Based Filtering)
        /// </summary>
        Task<TourDetailsViewModel?> GetTourDetailsAsync(int id);

        /// <summary>
        /// Lấy danh sách gợi ý tìm kiếm (autocomplete)
        /// </summary>
        Task<List<object>> GetSuggestionsAsync(string? term, int limit = 8);

        /// <summary>
        /// Chuẩn hóa chuỗi tiếng Việt (bỏ dấu, lowercase) để so sánh
        /// </summary>
        string NormalizeVietnamese(string? input);
    }
}
