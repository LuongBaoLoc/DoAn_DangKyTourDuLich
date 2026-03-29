namespace DoAn_DangKyTourDuLich.Middleware
{
    /// <summary>
    /// Middleware giới hạn tần suất submit review
    /// - Mỗi user tối đa 3 request/phút
    /// - Phòng chống spam API
    /// </summary>
    public class ReviewRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, List<DateTime>> _requestTracking = new();
        private static readonly object _lockObject = new object();

        private const int MAX_REQUESTS = 3;
        private const int WINDOW_MINUTES = 1;

        public ReviewRateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ áp dụng rate limiting cho request POST đến review endpoints
            if (context.Request.Method == "POST" && 
                (context.Request.Path.StartsWithSegments("/api/review") || 
                 context.Request.Path.StartsWithSegments("/Review")))
            {
                var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    if (!IsRateLimited(userId))
                    {
                        await _next(context);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new 
                        { 
                            error = $"Quá nhiều yêu cầu. Vui lòng đợi {WINDOW_MINUTES} phút trước khi thử lại" 
                        });
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private bool IsRateLimited(string userId)
        {
            lock (_lockObject)
            {
                var now = DateTime.UtcNow;
                var windowStart = now.AddMinutes(-WINDOW_MINUTES);

                if (!_requestTracking.ContainsKey(userId))
                {
                    _requestTracking[userId] = new List<DateTime> { now };
                    return false;
                }

                // Xóa các request cũ ngoài cửa sổ thời gian
                _requestTracking[userId] = _requestTracking[userId]
                    .Where(t => t > windowStart)
                    .ToList();

                // Kiểm tra số request
                if (_requestTracking[userId].Count >= MAX_REQUESTS)
                {
                    return true;
                }

                // Thêm request mới
                _requestTracking[userId].Add(now);
                return false;
            }
        }
    }

    /// <summary>
    /// Extension để dễ dàng đăng ký middleware
    /// Sử dụng: app.UseReviewRateLimiting()
    /// </summary>
    public static class ReviewRateLimitingExtensions
    {
        public static IApplicationBuilder UseReviewRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReviewRateLimitingMiddleware>();
        }
    }
}
