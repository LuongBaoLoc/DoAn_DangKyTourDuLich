using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdWithFullDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllWithDetailsAsync(OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .Include(o => o.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 10)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Tour)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Completed)
                .Select(o => (decimal?)o.TotalAmount)
                .SumAsync() ?? 0m;
        }

        public async Task<int> GetPendingOrderCountAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .CountAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
