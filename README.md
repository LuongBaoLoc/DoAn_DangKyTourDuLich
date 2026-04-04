# 🌍 Hệ Thống Đăng Ký Tour Du Lịch Online

Ứng dụng web đặt tour du lịch trực tuyến hiện đại, mạnh mẽ được xây dựng trên nền tảng **ASP.NET Core 10 MVC** kết hợp với **Entity Framework Core** và **SQL Server**. Dự án tập trung vào trải nghiệm người dùng (UX) với giao diện bắt mắt, hiệu ứng mượt mà và luồng booking tối ưu.

---

## 📋 Mục lục

- [✨ Tính năng nổi bật](#-tính-năng-nổi-bật)
- [🎨 Giao diện & Trải nghiệm (UX/UI)](#-giao-diện--trải-nghiệm-uxui)
- [🔧 Công nghệ & Thư viện](#-công-nghệ--thư-viện)
- [🏗️ Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [🚀 Hướng dẫn cài đặt & Chạy dự án](#-hướng-dẫn-cài-đặt--chạy-dự-án)
- [📁 Cấu trúc thư mục cốt lõi](#-cấu-trúc-thư-mục-cốt-lõi)
- [📡 API Documentation](#-api-documentation)
- [🧪 Testing](#-testing)

---

## ✨ Tính năng nổi bật

### 👤 Dành cho Khách Hàng
| Tính năng | Mô tả chi tiết |
|---|---|
| 🔐 **Authentication** | Hệ thống Đăng ký / Đăng nhập an toàn (Email/Password + Google/Facebook OAuth). |
| 🔍 **Tìm kiếm Tiên tiến** | Tìm kiếm theo keyword, danh mục, giá điểm đến. Hỗ trợ _Normalize_ dấu tiếng Việt (Tìm "Phu Quoc" ra "Phú Quốc"). |
| 📅 **Lịch Khởi Hành** | Calendar interactive trực quan cho phép người dùng chọn ngày giờ, xem độ trống chỗ. |
| 🛒 **Khách ghép / Đoàn riêng** | Tùy chọn đặt tour linh hoạt số lượng người lớn, trẻ em. Tính thành tiền tự động. |
| 💳 **Thanh toán Trực tuyến** | Tích hợp cổng thanh toán **VNPay** (Sandbox) an toàn, nhanh chóng. |
| ⭐ **Đánh giá (Review)** | Chỉ người dùng đã đi tour mới được đánh giá (1-5 sao). Hỗ trợ upload ảnh lên Cloudinary. |
| 📝 **Khảo sát (Survey)** | Hệ thống thu thập phản hồi khách hàng sau chuyến đi để nâng cao chất lượng dịch vụ. |
| 📄 **Hóa đơn & QR Code**| Xuất hóa đơn PDF chuyên nghiệp (QuestPDF) và gen QR Code quét mã vé điện tử. |
| 🎯 **Gợi ý Tour** | Hệ thống _Content-Based Filtering_ gợi ý tour tương tự dựa trên danh mục, giá, điểm đến. |

### 👨‍💼 Dành cho Ban Quản Trị (Admin)
| Tính năng | Mô tả chi tiết |
|---|---|
| 📊 **Dashboard** | Thống kê số liệu trực quan: Doanh thu, top tour bán chạy, đơn hàng gần đây. |
| 🏝️ **Quản lý Tour** | Tự do thêm/sửa/xóa Tour, upload nhiều ảnh thư viện tour cùng một lúc. |
| 📅 **Quản lý Lịch Trình** | Cài đặt ngày khởi hành cụ thể (Schedule), quản lý sỉ số tối đa từng chuyến. |
| 📋 **Quản lý Booking** | Theo dõi trạng thái đơn hàng: Chờ duyệt, Đã thanh toán, Hoàn thành, Đã hủy. |
| ✉️ **Hệ thống Email** | Gửi email tự động khi đăng ký thành công, xác nhận đơn, cấp vé QR Code và thông báo hoàn tiền. |

### 🛡️ Tính năng Bảo Mật & An Toàn
- **Role-based Auth:** Phân quyền chặt chẽ thông qua ASP.NET Core Identity.
- **Anti-XSS & CSRF:** Cơ chế AntiForgery Token ở mọi biểu mẫu POST, tích hợp _HtmlSanitization_ mạnh mẽ.
- **Rate Limiting:** Chống spam gọi API (đặc biệt đối với Review API).
- **Profanity Filter:** Tự động phát hiện và chặn các từ khóa cấm, tục tĩu tiếng Việt trong nhận xét.

---

## 🎨 Giao diện & Trải nghiệm (UX/UI)

Hệ thống được chăm chút đặc biệt về mặt hình ảnh nhằm mang lại cảm giác **"Wow"**, cao cấp và lôi cuốn nhất:
- **Glassmorphism Design**: Giao diện thẻ kính mờ sang trọng, bộ màu nền Gradient hiện đại.
- **Dynamic Animations**: Các hiệu ứng vi mô (micro-animations) trên thẻ tour, nút bấm. Nổi bật với hiệu ứng **Hoa đào rơi (Falling Petals)** tự nhiên theo CSS/JS trên toàn trang.
- **Typography & Màu sắc**: Phối màu HSL hòa âm, Font *Inter* hiện đại vượt qua quy chuẩn mặc định của trình duyệt.
- **Responsive**: Thích ứng hoàn hảo trên cả Di động, Tablet & Desktop.

---

## 🔧 Công nghệ & Thư viện

| Phân lớp | Công nghệ áp dụng |
|---|---|
| **Backend & Framework** | ASP.NET Core 10 MVC, C# |
| **Database & ORM** | SQL Server, Entity Framework Core 10 (Code First) |
| **Authentication** | ASP.NET Core Identity, Microsoft.AspNetCore.Authentication.(Google/Facebook) |
| **Payment Gateway** | Cổng thanh toán VNPay Sandbox |
| **Email Service** | MailKit, MimeKit (SMTP Gmail Transfer) |
| **Media & Storage** | Cloudinary (Lưu trữ ảnh Cloud) |
| **PDF & QR Code** | QuestPDF, QRCoder |
| **Logging** | Serilog (Structured Logging ra Console & File theo ngày) |
| **Frontend Utilities** | Bootstrap 5, jQuery, FontAwesome 6, Google Fonts |
| **Unit Testing** | xUnit, Moq, Entity Framework Core InMemory |

---

## 🏗️ Kiến trúc hệ thống

Dự án tuân thủ nghiêm ngặt chuẩn kiến trúc công nghiệp giúp dễ dàng mở rộng và bảo trì:

```text
┌─────────────────────────────────────────────────┐
│                   Views (Razor)                 │
│         Customer Views  │  Admin Area Views     │
├─────────────────────────────────────────────────┤
│                  Controllers                    │
│   TourController │ AccountController │ VnPay... │
├─────────────────────────────────────────────────┤
│               Services (Business Logic)         │
│  TourService │ SurveyService │ EmailService...  │
├─────────────────────────────────────────────────┤
│          Repositories + Unit of Work            │
│    ITourRepository  │  IOrderRepository         │
├─────────────────────────────────────────────────┤
│           Entity Framework Core                 │
│              ApplicationDbContext               │
├─────────────────────────────────────────────────┤
│                 SQL Server                      │
└─────────────────────────────────────────────────┘
```

**Các Design Patterns đã sử dụng:**
- **MVC (Model-View-Controller)**
- **Repository Pattern** — Cách ly tối đa Data Access Layer.
- **Unit of Work** — Quản lý gom nhóm các Transaction.
- **Dependency Injection (DI)** — Định tuyến lỏng lẻo (loose coupling) cho mọi class service.

---

## 🚀 Hướng dẫn cài đặt & Chạy dự án

### Yêu cầu hệ thống
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (phiên bản Developer hoặc Express đều được)
- Visual Studio 2022 hoặc VS Code.

### Các bước cài đặt

1. **Clone mã nguồn về máy**
   ```bash
   git clone https://github.com/your-username/DoAn_DangKyTourDuLich.git
   cd DoAn_DangKyTourDuLich
   ```

2. **Cập nhật chuỗi kết nối Database**
   Mở file `appsettings.json`, chỉnh sửa thuộc tính `DefaultConnection` sao cho phù hợp với SQL Server cục bộ của bạn:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=DoAn_DangKyTourDuLich;Trusted_Connection=True;Encrypt=False"
   }
   ```

3. **Cập nhật Database (EF Migrations)**
   Tiến hành tạo các bảng SQL bằng lệnh EF:
   ```bash
   cd DoAn_DangKyTourDuLich
   dotnet ef database update
   ```

4. **Khởi chạy ứng dụng**
   ```bash
   dotnet run
   ```

5. **Truy cập web**
   - Website chính thức tại cổng HTTPS: `https://localhost:7266` hoặc HTTP `http://localhost:5172`.
   - **Tài khoản Admin (Tự động Seed)**:
     - Email: `admin@tourdulich.com`
     - Mật khẩu: `Admin@123`

---

## 📁 Cấu trúc thư mục cốt lõi

```text
DoAn_DangKyTourDuLich/
├── Areas/Admin/              # Khu vực riêng của Quản trị viên
├── Controllers/              # Điều hướng logic Phía Client (Tour, Account, Review, VnPay, Survey)
├── Models/                   # Cấu trúc Entities & ViewModels (Tour, Order, Review, TourSchedule...)
├── Repositories/             # Triển khai Repository & UoW Pattern
├── Services/                 # Nơi tập trung toàn bộ Business Logic (Tách biệt khỏi Controller)
├── Middleware/               # Chứa custom middleware (như Rate Limiter)
├── Data/                     # ApplicationDbContext
├── Views/                    # Cấu trúc giao diện HTML (Razor/.cshtml)
├── wwwroot/                  # Tài nguyên tĩnh JS/CSS/Images.
│   └── js/falling-petals.js  # Script animation tạo hiệu ứng cánh hoa ấn tượng
├── Logs/                     # Vị trí tệp log lưu lại bằng Serilog
└── Program.cs                # Entry Point thiết lập Builder và Service Collections
```

---

## 📡 API Documentation

### Public API (Không yêu cầu đăng nhập)

- **GET** `/api/ToursApi`
  Lấy danh sách điểm đến.

- **GET** `/Tour/Suggestions?term={keyword}`
  Đề xuất keyword Tour theo Auto-complete bar tìm kiếm.

### Secure API

- **GET** `/api/review/can-review/{orderId}`
  Kiểm tra quyền hệ thống để biết user đã từng đặt chỗ và đi thành công để cấp quyền mở form Đánh giá thẻ sao.

---

## 🧪 Testing

Dự án áp dụng chặt chẽ văn hóa kiểm thử tự động, sử dụng **xUnit** kết hợp **Moq** và bộ giả lập **EF Core InMemory**.

Để chạy kiểm thử (Unit test):
```bash
dotnet test
```

### Độ phủ báo cáo (Test Coverage)
| Layer được Test | Tổng số Cases test | Đối tượng kiểm thử chính |
|---|---|---|
| **ProfanityFilterService** | 7 | Các tình huống phát hiện từ Spam bậy bạ, lọc vượt biên ký tự |
| **ReviewService** | 7 | Xác thực Business logic chặn nhận xét sai Order, Get list đơn đã đánh giá |
| **TourService** | 9 | Thuật toán Normalization dấu Unicode Tiếng Việt, Logic Recommend Items |

---

> **Lưu ý Bản quyền (License)**: Mã nguồn là đồ án học tập cá nhân, nghiên cứu giáo dục. Vui lòng không sử dụng thương mại mà không có sự cho phép.
