using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DoAn_DangKyTourDuLich.Models;
using System;

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

            // --- 1. CẤU HÌNH RÀNG BUỘC ---
            builder.Entity<Category>(entity => { entity.HasIndex(c => c.Name).IsUnique(); });

            builder.Entity<Tour>(entity => {
                entity.HasOne(t => t.Category).WithMany(c => c.Tours)
                      .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Order>(entity => {
                entity.HasIndex(o => o.OrderCode).IsUnique();
                entity.HasOne(o => o.User).WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<OrderDetail>(entity => {
                entity.HasOne(od => od.Order).WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Tour).WithMany(t => t.OrderDetails)
                      .HasForeignKey(od => od.TourId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Review>(entity => {
                entity.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(r => r.Tour).WithMany(t => t.Reviews).HasForeignKey(r => r.TourId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Booking).WithMany(o => o.Reviews).HasForeignKey(r => r.BookingId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(r => r.BookingId).IsUnique();
                entity.HasIndex(r => new { r.UserId, r.BookingId });
            });

            // --- 2. ĐỔ DỮ LIỆU MẪU ---
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Tour Trong Nước", IsActive = true, DisplayOrder = 1 },
                new Category { Id = 2, Name = "Tour Nước Ngoài", IsActive = true, DisplayOrder = 2 },
                new Category { Id = 3, Name = "Tour Biển Đảo", IsActive = true, DisplayOrder = 3 },
                new Category { Id = 4, Name = "Tour Núi Rừng", IsActive = true, DisplayOrder = 4 },
                new Category { Id = 5, Name = "Tour Văn Hóa", IsActive = true, DisplayOrder = 5 }
            );

            DateTime createdDate = new DateTime(2026, 4, 1);

            // ĐÃ THAY BẰNG 100% LINK ẢNH ONLINE (TỰ ĐỘNG HIỂN THỊ)
            builder.Entity<Tour>().HasData(
                new Tour
                {
                    Id = 1,
                    TourCode = "PQ001",
                    Name = "Phú Quốc: Nghỉ dưỡng Đảo Ngọc",
                    ShortDescription = "Vui chơi VinWonders & Safari.",
                    DetailDescription = "Tour nghỉ dưỡng cao cấp.",
                    Price = 5500000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Kiên Giang",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 5, 10),
                    MaxParticipants = 30,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1589782109277-516cd42b8bb7?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Sáng: Check-in Grand World - Xem show Tinh hoa Việt Nam | Trưa: Thưởng thức Bún Quậy Kiến Xây | Chiều: Vui chơi VinWonders",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 2,
                    TourCode = "PQ002",
                    Name = "Phú Quốc: Tour 4 Đảo Cano",
                    ShortDescription = "Lặn ngắm san hô & Check-in hòn Thơm.",
                    DetailDescription = "Tour mạo hiểm biển đảo.",
                    Price = 1800000,
                    DepartureLocation = "Phú Quốc",
                    Destination = "Kiên Giang",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 5, 12),
                    MaxParticipants = 20,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1557426272-fc759fdf7a8d?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Cano",
                    IsActive = true,
                    IsFeatured = false,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Sáng: Cano đi Hòn Mây Rút | Trưa: Ăn hải sản bè nổi | Chiều: Check-in Cầu Hôn (Kiss Bridge)",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 3,
                    TourCode = "DN001",
                    Name = "Đà Nẵng: Siêu phẩm Bà Nà Hills",
                    ShortDescription = "Check-in Cầu Vàng - Fantasy Park.",
                    DetailDescription = "Nghỉ dưỡng & Giải trí.",
                    Price = 4200000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Đà Nẵng",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 5, 1),
                    MaxParticipants = 40,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1559592442-741efca65ca6?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 1,
                    Schedule = "Sáng: Cáp treo Bà Nà - Cầu Vàng | Trưa: Buffet 100 món | Chiều: Vui chơi Fantasy Park",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 4,
                    TourCode = "DL001",
                    Name = "Đà Lạt: Thiên đường Sống ảo",
                    ShortDescription = "Check-in các vườn hoa & Cafe hot.",
                    DetailDescription = "Nghỉ dưỡng nhẹ nhàng.",
                    Price = 3200000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Lâm Đồng",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 5, 15),
                    MaxParticipants = 25,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1588665042459-7f79f220310e?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe du lịch",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 4,
                    Schedule = "Sáng: Vườn hoa Thành phố | Trưa: Ăn Lẩu gà lá é | Chiều: Cafe Still Cafe phong cách Nhật",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 5,
                    TourCode = "DL002",
                    Name = "Đà Lạt: Trekking Langbiang",
                    ShortDescription = "Chinh phục đỉnh núi - Săn mây.",
                    DetailDescription = "Mạo hiểm núi rừng.",
                    Price = 1500000,
                    DepartureLocation = "Đà Lạt",
                    Destination = "Lâm Đồng",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 5, 18),
                    MaxParticipants = 15,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1559599189-fe84dea4eb79?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe Jeep",
                    IsActive = true,
                    IsFeatured = false,
                    CreatedAt = createdDate,
                    CategoryId = 4,
                    Schedule = "Sáng: 04h00 Săn mây Cầu Đất | Trưa: Picnic đỉnh Langbiang | Chiều: Trượt thác Datanla",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = true,
                    IsForGroup = false
                },
                new Tour
                {
                    Id = 6,
                    TourCode = "SP001",
                    Name = "Sapa: Chinh phục Fansipan",
                    ShortDescription = "Cáp treo Fansipan - Bản Cát Cát.",
                    DetailDescription = "Chinh phục đỉnh cao.",
                    Price = 3800000,
                    DepartureLocation = "Hà Nội",
                    Destination = "Lào Cai",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 6, 1),
                    MaxParticipants = 25,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe giường nằm",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 4,
                    Schedule = "Sáng: Cáp treo Fansipan ngắm thung lũng | Trưa: Buffet trên núi | Chiều: Bản Cát Cát",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 7,
                    TourCode = "HL001",
                    Name = "Hạ Long: Du thuyền 5 sao",
                    ShortDescription = "Ngủ đêm trên vịnh di sản.",
                    DetailDescription = "Kỳ quan thiên nhiên.",
                    Price = 5500000,
                    DepartureLocation = "Hà Nội",
                    Destination = "Quảng Ninh",
                    Duration = 2,
                    DepartureDate = new DateTime(2026, 7, 1),
                    MaxParticipants = 40,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Du thuyền",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Ngày 1: Lên tàu - Tiệc Sunset trên boong | Trưa: Ăn hải sản | Chiều: Chèo Kayak",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 8,
                    TourCode = "NB001",
                    Name = "Ninh Bình: Tràng An - Hang Múa",
                    ShortDescription = "Chèo thuyền & Leo núi.",
                    DetailDescription = "Di sản kép thế giới.",
                    Price = 1500000,
                    DepartureLocation = "Hà Nội",
                    Destination = "Ninh Bình",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 5, 15),
                    MaxParticipants = 50,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1655712530188-75c1c4e97eb6?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe du lịch",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 4,
                    Schedule = "Sáng: Chinh phục Hang Múa ngắm lúa chín | Trưa: Đặc sản thịt dê | Chiều: Thuyền Tràng An",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 9,
                    TourCode = "HUE01",
                    Name = "Huế: Cố Đô Trầm Mặc",
                    ShortDescription = "Đại Nội - Lăng Tẩm - Sông Hương.",
                    DetailDescription = "Tìm về lịch sử.",
                    Price = 1200000,
                    DepartureLocation = "Huế",
                    Destination = "Thừa Thiên Huế",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 6, 1),
                    MaxParticipants = 30,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1590424744295-971268634782?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xích lô",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 5,
                    Schedule = "Sáng: Đại Nội - Chùa Thiên Mụ | Trưa: Cơm cung đình | Chiều: Lăng Khải Định",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 10,
                    TourCode = "HA001",
                    Name = "Hội An: Phố Cổ Lung Linh",
                    ShortDescription = "Đèn lồng - Thả hoa đăng.",
                    DetailDescription = "Lãng mạn - Hoài niệm.",
                    Price = 900000,
                    DepartureLocation = "Đà Nẵng",
                    Destination = "Quảng Nam",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 6, 15),
                    MaxParticipants = 50,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1555921015-5532091f6026?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Đi bộ",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 5,
                    Schedule = "Chiều: Thăm Chùa Cầu - Làng Gốm | Tối: Thả đèn hoa đăng sông Hoài - Cafe Faifo",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 11,
                    TourCode = "NT001",
                    Name = "Nha Trang: Nghỉ dưỡng Vinpearl",
                    ShortDescription = "Đảo Hòn Tre - Thiên đường vui chơi.",
                    DetailDescription = "Full dịch vụ.",
                    Price = 5200000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Khánh Hòa",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 7, 1),
                    MaxParticipants = 50,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1623941457173-0499e0df2438?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Ngày 1: Cáp treo ra đảo | Ngày 2: VinWonders - Show Nhạc nước | Ngày 3: Tắm bùn khoáng nóng",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 12,
                    TourCode = "MN001",
                    Name = "Mũi Né: Săn Hoàng Hôn Bàu Trắng",
                    ShortDescription = "Đồi cát trắng - Xe Jeep.",
                    DetailDescription = "Check-in cực hot.",
                    Price = 1500000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Bình Thuận",
                    Duration = 2,
                    DepartureDate = new DateTime(2026, 8, 1),
                    MaxParticipants = 30,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1571508601936-6ca847b47ae4?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe du lịch",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Sáng: Săn bình minh đồi cát | Trưa: Ăn bánh xèo | Chiều: Xe Jeep địa hình Bàu Trắng",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 13,
                    TourCode = "HCM01",
                    Name = "Sài Gòn: Ngắm sông từ Bitexco",
                    ShortDescription = "Dinh Độc Lập - Bưu Điện - Landmark.",
                    DetailDescription = "Sài Gòn hoa lệ.",
                    Price = 1100000,
                    DepartureLocation = "TP.HCM",
                    Destination = "TP. Hồ Chí Minh",
                    Duration = 1,
                    DepartureDate = new DateTime(2026, 4, 10),
                    MaxParticipants = 40,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1583417311718-c29e46a7be7f?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe buýt 2 tầng",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 1,
                    Schedule = "Sáng: Dinh Độc Lập - Nhà Thờ Đức Bà | Trưa: Cơm tấm | Chiều: Landmark 81",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 14,
                    TourCode = "VT001",
                    Name = "Vũng Tàu: Nghỉ dưỡng The Imperial",
                    ShortDescription = "Khách sạn phong cách Victoria.",
                    DetailDescription = "Lộng lẫy - Cổ điển.",
                    Price = 4500000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Bà Rịa - Vũng Tàu",
                    Duration = 2,
                    DepartureDate = new DateTime(2026, 5, 1),
                    MaxParticipants = 20,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1563298723-dcfebaa392e3?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe du lịch",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 3,
                    Schedule = "Ngày 1: Đón khách - Tea party | Ngày 2: Tắm biển riêng - Thăm Bạch Dinh",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 15,
                    TourCode = "CT001",
                    Name = "Cần Thơ: Chợ Nổi & Miệt Vườn",
                    ShortDescription = "Cái Răng - Mỹ Khánh - Sông nước.",
                    DetailDescription = "Văn hóa miền Tây.",
                    Price = 1200000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Cần Thơ",
                    Duration = 2,
                    DepartureDate = new DateTime(2026, 6, 1),
                    MaxParticipants = 50,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1599321451299-a1b7e466ce78?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Xe du lịch",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 5,
                    Schedule = "Ngày 1: Thăm nhà cổ Bình Thủy | Ngày 2: 05h00 Chợ nổi Cái Răng - Vườn Mỹ Khánh",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = true,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 16,
                    TourCode = "THAI1",
                    Name = "Thái Lan: Nghỉ dưỡng Pattaya biển xanh",
                    ShortDescription = "Đảo San Hô - Show chuyển giới Alcazar.",
                    DetailDescription = "Tour du lịch giải trí.",
                    Price = 6990000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Thái Lan",
                    Duration = 5,
                    DepartureDate = new DateTime(2026, 6, 15),
                    MaxParticipants = 30,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1504214208698-ea1919a2f9e0?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 2,
                    Schedule = "Sáng: Bay đến Bangkok | Trưa: Lẩu Thái Tomyum | Chiều: Tắm biển Đảo San Hô | Tối: Xem show Alcazar",
                    IsBeach = true,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 17,
                    TourCode = "JPN01",
                    Name = "Nhật Bản: Nghỉ dưỡng Núi Phú Sĩ",
                    ShortDescription = "Tắm Onsen - Ngắm núi Phú Sĩ.",
                    DetailDescription = "Kỳ nghỉ đẳng cấp.",
                    Price = 28900000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Nhật Bản",
                    Duration = 6,
                    DepartureDate = new DateTime(2026, 4, 10),
                    MaxParticipants = 20,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 2,
                    Schedule = "Sáng: Thăm làng cổ Oshino Hakkai | Trưa: Mì Ramen | Chiều: Tắm khoáng nóng Onsen ngắm Núi Phú Sĩ",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 18,
                    TourCode = "KOR01",
                    Name = "Hàn Quốc: Nghỉ dưỡng Đảo Nami",
                    ShortDescription = "Phim trường lãng mạn - Nami Island.",
                    DetailDescription = "Mùa thu vàng rực rỡ.",
                    Price = 15900000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Hàn Quốc",
                    Duration = 5,
                    DepartureDate = new DateTime(2026, 10, 15),
                    MaxParticipants = 25,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 2,
                    Schedule = "Sáng: Di chuyển đến đảo Nami | Trưa: Gà nướng cay | Chiều: Check-in hàng cây ngân hạnh",
                    IsBeach = false,
                    IsMountain = true,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 19,
                    TourCode = "SIN01",
                    Name = "Singapore: Nghỉ dưỡng Marina Bay Sands",
                    ShortDescription = "Hồ bơi vô cực - Garden by the Bay.",
                    DetailDescription = "Trải nghiệm sang trọng.",
                    Price = 12500000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Singapore",
                    Duration = 3,
                    DepartureDate = new DateTime(2026, 5, 20),
                    MaxParticipants = 25,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1525625293386-3f8f99389edd?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 2,
                    Schedule = "Sáng: Thăm Garden by the Bay | Trưa: Cơm gà Hải Nam | Chiều: Tắm hồ bơi vô cực cao nhất thế giới",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                },
                new Tour
                {
                    Id = 20,
                    TourCode = "FRA01",
                    Name = "Pháp: Nghỉ dưỡng bên dòng sông Seine",
                    ShortDescription = "Tháp Eiffel - Bảo tàng Louvre.",
                    DetailDescription = "Tour lãng mạn mộng mơ.",
                    Price = 65000000,
                    DepartureLocation = "TP.HCM",
                    Destination = "Pháp",
                    Duration = 7,
                    DepartureDate = new DateTime(2026, 9, 10),
                    MaxParticipants = 15,
                    CurrentParticipants = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?auto=format&fit=crop&w=800&q=80",
                    Transportation = "Máy bay",
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = createdDate,
                    CategoryId = 2,
                    Schedule = "Sáng: Thăm Tháp Eiffel | Trưa: Ăn món Pháp | Chiều: Du thuyền sông Seine | Tối: Rượu vang Pháp",
                    IsBeach = false,
                    IsMountain = false,
                    IsLowBudget = false,
                    IsForGroup = true
                }
            );
        }
    }
}