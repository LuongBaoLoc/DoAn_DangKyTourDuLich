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
        public DbSet<TourSchedule> TourSchedules { get; set; } = null!;

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
                      
                entity.HasMany(t => t.TourSchedules)
                      .WithOne(ts => ts.Tour)
                      .HasForeignKey(ts => ts.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
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

            // Cấu hình Review
            builder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Tour)
                      .WithMany(t => t.Reviews)
                      .HasForeignKey(r => r.TourId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Booking)
                      .WithMany(o => o.Reviews)
                      .HasForeignKey(r => r.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.BookingId).IsUnique();
                entity.HasIndex(r => new { r.UserId, r.BookingId });
            });
        }
    }
}
