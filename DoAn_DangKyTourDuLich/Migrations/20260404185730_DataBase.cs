using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAn_DangKyTourDuLich.Migrations
{
    /// <inheritdoc />
    public partial class DataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetailDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    DepartureLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    CurrentParticipants = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrlsData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Transportation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Schedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsBeach = table.Column<bool>(type: "bit", nullable: false),
                    IsMountain = table.Column<bool>(type: "bit", nullable: false),
                    IsForGroup = table.Column<bool>(type: "bit", nullable: false),
                    IsLowBudget = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImagesData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    HideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Orders_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "DisplayOrder", "ImageUrl", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, null, 1, null, true, "Tour Trong Nước" },
                    { 2, null, 2, null, true, "Tour Nước Ngoài" },
                    { 3, null, 3, null, true, "Tour Biển Đảo" },
                    { 4, null, 4, null, true, "Tour Núi Rừng" },
                    { 5, null, 5, null, true, "Tour Văn Hóa" }
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "CurrentParticipants", "DepartureDate", "DepartureLocation", "Destination", "DetailDescription", "DiscountPrice", "Duration", "ImageUrl", "ImageUrlsData", "IsActive", "IsBeach", "IsFeatured", "IsForGroup", "IsLowBudget", "IsMountain", "MaxParticipants", "Name", "Price", "Schedule", "ShortDescription", "TourCode", "Transportation", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Kiên Giang", "Tour nghỉ dưỡng cao cấp.", null, 3, "https://images.unsplash.com/photo-1589782109277-516cd42b8bb7?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 30, "Phú Quốc: Nghỉ dưỡng Đảo Ngọc", 5500000m, "Sáng: Check-in Grand World - Xem show Tinh hoa Việt Nam | Trưa: Thưởng thức Bún Quậy Kiến Xây | Chiều: Vui chơi VinWonders", "Vui chơi VinWonders & Safari.", "PQ001", "Máy bay", null },
                    { 2, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phú Quốc", "Kiên Giang", "Tour mạo hiểm biển đảo.", null, 1, "https://images.unsplash.com/photo-1557426272-fc759fdf7a8d?auto=format&fit=crop&w=800&q=80", null, true, true, false, true, true, false, 20, "Phú Quốc: Tour 4 Đảo Cano", 1800000m, "Sáng: Cano đi Hòn Mây Rút | Trưa: Ăn hải sản bè nổi | Chiều: Check-in Cầu Hôn (Kiss Bridge)", "Lặn ngắm san hô & Check-in hòn Thơm.", "PQ002", "Cano", null },
                    { 3, 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Đà Nẵng", "Nghỉ dưỡng & Giải trí.", null, 3, "https://images.unsplash.com/photo-1559592442-741efca65ca6?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 40, "Đà Nẵng: Siêu phẩm Bà Nà Hills", 4200000m, "Sáng: Cáp treo Bà Nà - Cầu Vàng | Trưa: Buffet 100 món | Chiều: Vui chơi Fantasy Park", "Check-in Cầu Vàng - Fantasy Park.", "DN001", "Máy bay", null },
                    { 4, 4, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Lâm Đồng", "Nghỉ dưỡng nhẹ nhàng.", null, 3, "https://images.unsplash.com/photo-1588665042459-7f79f220310e?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, true, 25, "Đà Lạt: Thiên đường Sống ảo", 3200000m, "Sáng: Vườn hoa Thành phố | Trưa: Ăn Lẩu gà lá é | Chiều: Cafe Still Cafe phong cách Nhật", "Check-in các vườn hoa & Cafe hot.", "DL001", "Xe du lịch", null },
                    { 5, 4, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đà Lạt", "Lâm Đồng", "Mạo hiểm núi rừng.", null, 1, "https://images.unsplash.com/photo-1559599189-fe84dea4eb79?auto=format&fit=crop&w=800&q=80", null, true, false, false, false, true, true, 15, "Đà Lạt: Trekking Langbiang", 1500000m, "Sáng: 04h00 Săn mây Cầu Đất | Trưa: Picnic đỉnh Langbiang | Chiều: Trượt thác Datanla", "Chinh phục đỉnh núi - Săn mây.", "DL002", "Xe Jeep", null },
                    { 6, 4, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hà Nội", "Lào Cai", "Chinh phục đỉnh cao.", null, 3, "https://images.unsplash.com/photo-1504457047772-27faf1c00561?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, true, 25, "Sapa: Chinh phục Fansipan", 3800000m, "Sáng: Cáp treo Fansipan ngắm thung lũng | Trưa: Buffet trên núi | Chiều: Bản Cát Cát", "Cáp treo Fansipan - Bản Cát Cát.", "SP001", "Xe giường nằm", null },
                    { 7, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hà Nội", "Quảng Ninh", "Kỳ quan thiên nhiên.", null, 2, "https://images.unsplash.com/photo-1528127269322-539801943592?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 40, "Hạ Long: Du thuyền 5 sao", 5500000m, "Ngày 1: Lên tàu - Tiệc Sunset trên boong | Trưa: Ăn hải sản | Chiều: Chèo Kayak", "Ngủ đêm trên vịnh di sản.", "HL001", "Du thuyền", null },
                    { 8, 4, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hà Nội", "Ninh Bình", "Di sản kép thế giới.", null, 1, "https://images.unsplash.com/photo-1655712530188-75c1c4e97eb6?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, true, 50, "Ninh Bình: Tràng An - Hang Múa", 1500000m, "Sáng: Chinh phục Hang Múa ngắm lúa chín | Trưa: Đặc sản thịt dê | Chiều: Thuyền Tràng An", "Chèo thuyền & Leo núi.", "NB001", "Xe du lịch", null },
                    { 9, 5, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Huế", "Thừa Thiên Huế", "Tìm về lịch sử.", null, 1, "https://images.unsplash.com/photo-1590424744295-971268634782?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, false, 30, "Huế: Cố Đô Trầm Mặc", 1200000m, "Sáng: Đại Nội - Chùa Thiên Mụ | Trưa: Cơm cung đình | Chiều: Lăng Khải Định", "Đại Nội - Lăng Tẩm - Sông Hương.", "HUE01", "Xích lô", null },
                    { 10, 5, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đà Nẵng", "Quảng Nam", "Lãng mạn - Hoài niệm.", null, 1, "https://images.unsplash.com/photo-1555921015-5532091f6026?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, false, 50, "Hội An: Phố Cổ Lung Linh", 900000m, "Chiều: Thăm Chùa Cầu - Làng Gốm | Tối: Thả đèn hoa đăng sông Hoài - Cafe Faifo", "Đèn lồng - Thả hoa đăng.", "HA001", "Đi bộ", null },
                    { 11, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Khánh Hòa", "Full dịch vụ.", null, 3, "https://images.unsplash.com/photo-1623941457173-0499e0df2438?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 50, "Nha Trang: Nghỉ dưỡng Vinpearl", 5200000m, "Ngày 1: Cáp treo ra đảo | Ngày 2: VinWonders - Show Nhạc nước | Ngày 3: Tắm bùn khoáng nóng", "Đảo Hòn Tre - Thiên đường vui chơi.", "NT001", "Máy bay", null },
                    { 12, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Bình Thuận", "Check-in cực hot.", null, 2, "https://images.unsplash.com/photo-1571508601936-6ca847b47ae4?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, true, false, 30, "Mũi Né: Săn Hoàng Hôn Bàu Trắng", 1500000m, "Sáng: Săn bình minh đồi cát | Trưa: Ăn bánh xèo | Chiều: Xe Jeep địa hình Bàu Trắng", "Đồi cát trắng - Xe Jeep.", "MN001", "Xe du lịch", null },
                    { 13, 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "TP. Hồ Chí Minh", "Sài Gòn hoa lệ.", null, 1, "https://images.unsplash.com/photo-1583417311718-c29e46a7be7f?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, false, 40, "Sài Gòn: Ngắm sông từ Bitexco", 1100000m, "Sáng: Dinh Độc Lập - Nhà Thờ Đức Bà | Trưa: Cơm tấm | Chiều: Landmark 81", "Dinh Độc Lập - Bưu Điện - Landmark.", "HCM01", "Xe buýt 2 tầng", null },
                    { 14, 3, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Bà Rịa - Vũng Tàu", "Lộng lẫy - Cổ điển.", null, 2, "https://images.unsplash.com/photo-1563298723-dcfebaa392e3?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 20, "Vũng Tàu: Nghỉ dưỡng The Imperial", 4500000m, "Ngày 1: Đón khách - Tea party | Ngày 2: Tắm biển riêng - Thăm Bạch Dinh", "Khách sạn phong cách Victoria.", "VT001", "Xe du lịch", null },
                    { 15, 5, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Cần Thơ", "Văn hóa miền Tây.", null, 2, "https://images.unsplash.com/photo-1599321451299-a1b7e466ce78?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, true, false, 50, "Cần Thơ: Chợ Nổi & Miệt Vườn", 1200000m, "Ngày 1: Thăm nhà cổ Bình Thủy | Ngày 2: 05h00 Chợ nổi Cái Răng - Vườn Mỹ Khánh", "Cái Răng - Mỹ Khánh - Sông nước.", "CT001", "Xe du lịch", null },
                    { 16, 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Thái Lan", "Tour du lịch giải trí.", null, 5, "https://images.unsplash.com/photo-1504214208698-ea1919a2f9e0?auto=format&fit=crop&w=800&q=80", null, true, true, true, true, false, false, 30, "Thái Lan: Nghỉ dưỡng Pattaya biển xanh", 6990000m, "Sáng: Bay đến Bangkok | Trưa: Lẩu Thái Tomyum | Chiều: Tắm biển Đảo San Hô | Tối: Xem show Alcazar", "Đảo San Hô - Show chuyển giới Alcazar.", "THAI1", "Máy bay", null },
                    { 17, 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Nhật Bản", "Kỳ nghỉ đẳng cấp.", null, 6, "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, false, true, 20, "Nhật Bản: Nghỉ dưỡng Núi Phú Sĩ", 28900000m, "Sáng: Thăm làng cổ Oshino Hakkai | Trưa: Mì Ramen | Chiều: Tắm khoáng nóng Onsen ngắm Núi Phú Sĩ", "Tắm Onsen - Ngắm núi Phú Sĩ.", "JPN01", "Máy bay", null },
                    { 18, 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Hàn Quốc", "Mùa thu vàng rực rỡ.", null, 5, "https://images.unsplash.com/photo-1538481199705-c710c4e965fc?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, false, true, 25, "Hàn Quốc: Nghỉ dưỡng Đảo Nami", 15900000m, "Sáng: Di chuyển đến đảo Nami | Trưa: Gà nướng cay | Chiều: Check-in hàng cây ngân hạnh", "Phim trường lãng mạn - Nami Island.", "KOR01", "Máy bay", null },
                    { 19, 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Singapore", "Trải nghiệm sang trọng.", null, 3, "https://images.unsplash.com/photo-1525625293386-3f8f99389edd?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, false, false, 25, "Singapore: Nghỉ dưỡng Marina Bay Sands", 12500000m, "Sáng: Thăm Garden by the Bay | Trưa: Cơm gà Hải Nam | Chiều: Tắm hồ bơi vô cực cao nhất thế giới", "Hồ bơi vô cực - Garden by the Bay.", "SIN01", "Máy bay", null },
                    { 20, 2, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "TP.HCM", "Pháp", "Tour lãng mạn mộng mơ.", null, 7, "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?auto=format&fit=crop&w=800&q=80", null, true, false, true, true, false, false, 15, "Pháp: Nghỉ dưỡng bên dòng sông Seine", 65000000m, "Sáng: Thăm Tháp Eiffel | Trưa: Ăn món Pháp | Chiều: Du thuyền sông Seine | Tối: Rượu vang Pháp", "Tháp Eiffel - Bảo tàng Louvre.", "FRA01", "Máy bay", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TourId",
                table: "OrderDetails",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderCode",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TourId",
                table: "Reviews",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_BookingId",
                table: "Reviews",
                columns: new[] { "UserId", "BookingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_CategoryId",
                table: "Tours",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
