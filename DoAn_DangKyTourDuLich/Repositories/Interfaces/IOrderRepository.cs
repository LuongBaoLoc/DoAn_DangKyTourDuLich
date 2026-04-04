using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<Order?> GetByIdWithFullDetailsAsync(int id);
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<List<Order>> GetAllWithDetailsAsync(OrderStatus? status = null);
        Task<List<Order>> GetRecentOrdersAsync(int count = 10);
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetPendingOrderCountAsync();
        Task AddAsync(Order order);
        void Update(Order order);
    }
}
