# 🌍 Hệ Thống Đăng Ký Tour Du Lịch Online

Ứng dụng web đặt tour du lịch trực tuyến xây dựng trên nền tảng **ASP.NET Core 10 MVC** với Entity Framework Core và SQL Server.

## 📋 Mục lục

- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Screenshots](#-screenshots)

---

## ✨ Tính năng

### 👤 Phía khách hàng
| Tính năng | Mô tả |
|---|---|
| 🔐 Đăng ký / Đăng nhập | Email + Google + Facebook OAuth |
| 🔍 Tìm kiếm Tour | Theo keyword, danh mục, giá, thời gian, điểm đến |
| 🇻🇳 Tìm kiếm tiếng Việt | Normalize dấu Unicode, search không dấu |
| 📅 Lịch khởi hành | Calendar interactive chọn ngày + giờ |
| 🛒 Đặt tour | Tour ghép / Đoàn riêng, người lớn / trẻ em |
| 💳 Thanh toán VNPay | Cổng thanh toán online sandbox |
| ⭐ Đánh giá tour | Rating 1-5 sao, upload ảnh lên Cloudinary |
| 📄 Xuất hóa đơn PDF | QuestPDF — hóa đơn chuyên nghiệp |
| 🎯 Gợi ý tour | Content-Based Filtering (danh mục + giá + điểm đến) |

### 👨‍💼 Phía quản trị (Admin)
| Tính năng | Mô tả |
|---|---|
| 📊 Dashboard | Thống kê doanh thu, top tour, đơn gần đây |
| 🏝️ Quản lý Tour | CRUD tour + upload nhiều ảnh |
| 📋 Quản lý đơn hàng | Xác nhận / Hoàn thành / Hủy đơn |
| 📂 Quản lý danh mục | CRUD danh mục tour |
| 📅 Quản lý lịch trình | Thêm/xóa ngày khởi hành cho từng tour |
| ✉️ Email tự động | Xác nhận booking + QR Code + Hoàn tiền |

### 🛡️ Bảo mật
- ASP.NET Core Identity với phân quyền Role-based (Admin/Customer)
- AntiForgery Token trên tất cả form POST
- HTML Sanitization chống XSS
- Rate Limiting cho review API
- Profanity Filter lọc từ cấm tiếng Việt

---

## 🔧 Công nghệ sử dụng

| Layer | Công nghệ |
|---|---|
| **Backend** | ASP.NET Core 10 MVC, C# |
| **ORM** | Entity Framework Core 10 (Code First) |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Core Identity + Google/Facebook OAuth |
| **Payment** | VNPay Sandbox API |
| **Email** | MailKit + SMTP Gmail |
| **Cloud Storage** | Cloudinary (ảnh review) |
| **PDF** | QuestPDF |
| **QR Code** | QRCoder |
| **Logging** | Serilog (Console + File) |
| **Frontend** | Bootstrap 5, jQuery, Font Awesome, Google Fonts (Inter) |
| **Testing** | xUnit, Moq, EF Core InMemory |

---

## 🏗️ Kiến trúc hệ thống

```
┌─────────────────────────────────────────────────┐
│                   Views (Razor)                  │
│         Customer Views  │  Admin Area Views      │
├─────────────────────────────────────────────────┤
│                  Controllers                     │
│   TourController │ AccountController │ VnPay...  │
├─────────────────────────────────────────────────┤
│               Services (Business Logic)          │
│  TourService │ ReviewService │ EmailService │ .. │
├─────────────────────────────────────────────────┤
│          Repositories + Unit of Work             │
│    ITourRepository  │  IOrderRepository          │
├─────────────────────────────────────────────────┤
│           Entity Framework Core                  │
│              ApplicationDbContext                 │
├─────────────────────────────────────────────────┤
│                 SQL Server                       │
└─────────────────────────────────────────────────┘
```

**Design Patterns áp dụng:**
- **MVC Pattern** — Tách biệt Model, View, Controller
- **Repository Pattern** — Đóng gói data access logic
- **Unit of Work** — Quản lý transaction scope
- **Dependency Injection** — Tất cả service đăng ký qua DI container
- **Service Layer** — Business logic tách khỏi controller

---

## 🚀 Cài đặt và chạy

### Yêu cầu
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express hoặc Developer)
- Visual Studio 2022+ hoặc VS Code

### Các bước

1. **Clone repository**
```bash
git clone https://github.com/your-username/DoAn_DangKyTourDuLich.git
cd DoAn_DangKyTourDuLich
```

2. **Cấu hình database** — Sửa `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DoAn_DangKyTourDuLich;Trusted_Connection=True;Encrypt=False"
}
```

3. **Chạy migration**
```bash
cd DoAn_DangKyTourDuLich
dotnet ef database update
```

4. **Chạy ứng dụng**
```bash
dotnet run
```

5. **Truy cập**
- Website: `https://localhost:7001`
- Admin: `admin@tourdulich.com` / `Admin@123`

---

## 📁 Cấu trúc thư mục

```
DoAn_DangKyTourDuLich/
├── Areas/Admin/              # Admin area (Dashboard, Tour, Order, Category)
│   ├── Controllers/
│   └── Views/
├── Controllers/              # Customer-facing controllers
│   ├── TourController.cs     # Đặt tour, tìm kiếm (refactored → thin)
│   ├── AccountController.cs  # Auth + OAuth
│   ├── ReviewController.cs   # Hệ thống đánh giá
│   ├── VnPayController.cs    # Thanh toán VNPay
│   └── Api/                  # REST API
├── Models/
│   ├── Tour.cs, Order.cs, User.cs, Review.cs, Category.cs
│   ├── TourSchedule.cs
│   └── ViewModels/
├── Repositories/             # Repository + Unit of Work Pattern
│   ├── Interfaces/           # ITourRepository, IOrderRepository, IUnitOfWork 
│   ├── TourRepository.cs
│   ├── OrderRepository.cs
│   └── UnitOfWork.cs
├── Services/                 # Business Logic Layer
│   ├── Interfaces/           # ITourService, IVnPayService
│   ├── TourService.cs        # Search, Recommendations, Vietnamese normalize
│   ├── VnPayService.cs       # Cổng thanh toán VNPay
│   ├── EmailService.cs       # Email xác nhận + QR Code
│   ├── ReviewService.cs      # Logic đánh giá
│   ├── PdfInvoiceService.cs  # Xuất hóa đơn PDF
│   ├── CloudinaryService.cs  # Upload ảnh cloud
│   ├── ProfanityFilterService.cs  # Lọc từ cấm
│   └── QRCodeService.cs
├── Middleware/
│   └── ReviewRateLimitingMiddleware.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/                    # Razor Views
├── wwwroot/                  # Static files (CSS, JS, images)
├── Migrations/
├── Logs/                     # Serilog log files
└── Program.cs                # Entry point + DI configuration
```

---

## 📡 API Documentation

### Public API (không cần auth)

#### GET `/api/ToursApi`
Lấy danh sách tất cả tour đang active.
```json
[
  {
    "id": 1,
    "name": "Tour Phú Quốc 3N2Đ",
    "tourCode": "T26041234",
    "categoryName": "Biển đảo",
    "displayPrice": 5000000,
    "availableSlots": 25,
    "duration": 3,
    "destination": "Phú Quốc"
  }
]
```

#### GET `/api/ToursApi/{id}`
Lấy chi tiết tour kèm lịch khởi hành.

#### GET `/Tour/Suggestions?term={keyword}`
Gợi ý tìm kiếm tour (autocomplete).

### Authenticated API

#### GET `/api/review/can-review/{orderId}`
Kiểm tra xem user có thể đánh giá đơn hàng không.

---

## 🧪 Testing

Dự án sử dụng **xUnit** với **Moq** và **EF Core InMemory**.

```bash
# Chạy tất cả tests
dotnet test

# Chạy với output chi tiết
dotnet test --verbosity normal
```

### Test coverage:
| Service | Tests | Mô tả |
|---|---|---|
| `ProfanityFilterService` | 7 tests | Lọc từ cấm, spam, độ dài |
| `ReviewService` | 7 tests | CanReview logic, GetCompletedOrders |
| `TourService` | 9 tests | Search, normalize Vietnamese, recommendations |

---

## 👨‍💻 Tác giả

**Lương Bảo Lộc**
- Email: luongbaoloc2014@gmail.com
- Facebook: [LuongBaoLoc2K5](https://www.facebook.com/LuongBaoLoc2K5)

---

## 📄 License

Đồ án môn học — Không sử dụng cho mục đích thương mại.
