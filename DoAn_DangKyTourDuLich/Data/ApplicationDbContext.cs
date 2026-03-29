using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình Category
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // Cấu hình Tour
            builder.Entity<Tour>(entity =>
            {
                entity.HasOne(t => t.Category)
                      .WithMany(c => c.Tours)
                      .HasForeignKey(t => t.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình Order
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(o => o.OrderCode).IsUnique();
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Cấu hình OrderDetail
            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Tour)
                      .WithMany(t => t.OrderDetails)
                      .HasForeignKey(od => od.TourId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
