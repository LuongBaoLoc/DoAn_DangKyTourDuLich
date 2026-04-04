using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Services;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using DoAn_DangKyTourDuLich.Repositories;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using DoAn_DangKyTourDuLich.Middleware;
using Serilog;

// ═══════════════════════════════════════════════════════════════
// Cấu hình Serilog — Structured Logging
// ═══════════════════════════════════════════════════════════════
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Sử dụng Serilog thay cho built-in logging
builder.Host.UseSerilog();

// ═══════════════════════════════════════════════════════════════
// Đăng ký Services (Business Logic Layer)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<QRCodeService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<ProfanityFilterService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<PdfInvoiceService>();
builder.Services.AddSingleton<DoAn_DangKyTourDuLich.Services.HtmlSanitizeService>();

// Service với Interface (dễ test, dễ mock)
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

// ═══════════════════════════════════════════════════════════════
// Đăng ký Repository + Unit of Work Pattern
// ═══════════════════════════════════════════════════════════════
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ═══════════════════════════════════════════════════════════════
// Cấu hình MVC + Entity Framework + Identity
// ═══════════════════════════════════════════════════════════════
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ═══════════════════════════════════════════════════════════════
// Cấu hình Authentication (Google + Facebook)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"] ?? string.Empty;
        options.ClientSecret = googleAuthNSection["ClientSecret"] ?? string.Empty;
    })
    .AddFacebook(options =>
    {
        IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
        options.AppId = facebookAuthNSection["AppId"] ?? string.Empty;
        options.AppSecret = facebookAuthNSection["AppSecret"] ?? string.Empty;
    });

// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════
// Seed roles và tài khoản Admin mặc định
// ═══════════════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "admin@tourdulich.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Quản trị viên",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        Log.Information("Seed dữ liệu ban đầu thành công");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Lỗi khi seed dữ liệu ban đầu");
    }
}

// ═══════════════════════════════════════════════════════════════
// Cấu hình HTTP Request Pipeline
// ═══════════════════════════════════════════════════════════════
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Thêm Serilog request logging
app.UseSerilogRequestLogging();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Middleware chống spam review
app.UseReviewRateLimiting();

app.MapStaticAssets();

// Route cho Area Admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    Log.Information("Ứng dụng Tour Du Lịch khởi chạy");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Ứng dụng bị crash khi khởi chạy");
}
finally
{
    Log.CloseAndFlush();
}
