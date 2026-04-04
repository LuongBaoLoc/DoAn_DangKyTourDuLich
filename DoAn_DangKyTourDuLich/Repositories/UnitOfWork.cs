using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;

namespace DoAn_DangKyTourDuLich.Repositories
{
    /// <summary>
    /// Unit of Work pattern — quản lý lifecycle của DbContext,
    /// đảm bảo tất cả repositories chia sẻ cùng một transaction.
    /// Gọi SaveChangesAsync() một lần để commit toàn bộ thay đổi.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITourRepository? _tourRepository;
        private IOrderRepository? _orderRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITourRepository Tours =>
            _tourRepository ??= new TourRepository(_context);

        public IOrderRepository Orders =>
            _orderRepository ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
