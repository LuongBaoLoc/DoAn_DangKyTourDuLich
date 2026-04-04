namespace DoAn_DangKyTourDuLich.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work pattern — đảm bảo tất cả repository chia sẻ cùng DbContext
    /// và commit transaction một lần duy nhất.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        ITourRepository Tours { get; }
        IOrderRepository Orders { get; }
        Task<int> SaveChangesAsync();
    }
}
