using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_DangKyTourDuLich.Controllers
{
    /// <summary>
    /// Controller xử lý thanh toán qua cổng VNPay.
    /// Flow: Tạo URL → Redirect khách → VNPay callback → Cập nhật đơn hàng
    /// </summary>
    [Authorize]
    public class VnPayController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VnPayController> _logger;

        public VnPayController(IVnPayService vnPayService, IUnitOfWork unitOfWork, ILogger<VnPayController> logger)
        {
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Tạo link thanh toán VNPay và redirect khách hàng đến cổng thanh toán
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            if (order == null) return NotFound();

            var tourName = order.OrderDetails?.FirstOrDefault()?.Tour?.Name ?? "Tour du lịch";
            var orderInfo = $"Thanh toan don hang {order.OrderCode} - {tourName}";

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var paymentUrl = _vnPayService.CreatePaymentUrl(order.Id, order.TotalAmount, orderInfo, ipAddress);

            _logger.LogInformation("Redirect khách {CustomerName} đến VNPay cho đơn {OrderCode}",
                order.CustomerName, order.OrderCode);

            return Redirect(paymentUrl);
        }

        /// <summary>
        /// VNPay callback — xử lý kết quả thanh toán từ VNPay
        /// URL này được cấu hình trong appsettings.json > VnPay:ReturnUrl
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.ProcessPaymentResponse(Request.Query);

            if (!int.TryParse(response.OrderId, out var orderId))
            {
                _logger.LogWarning("VNPay callback: OrderId không hợp lệ: {OrderId}", response.OrderId);
                TempData["Error"] = "Thông tin thanh toán không hợp lệ.";
                return RedirectToAction("Index", "Home");
            }

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index", "Home");
            }

            if (response.Success)
            {
                order.Status = OrderStatus.Pending; // Chờ admin xác nhận
                order.UpdatedAt = DateTime.UtcNow;
                order.Note = (order.Note ?? "") + $"\n[VNPay] Thanh toán thành công - Mã GD: {response.TransactionId}";

                _logger.LogInformation("VNPay: Thanh toán thành công cho đơn {OrderCode}, Mã GD: {TransactionId}",
                    order.OrderCode, response.TransactionId);

                TempData["Success"] = $"Thanh toán thành công! Mã giao dịch: {response.TransactionId}";
            }
            else
            {
                order.Note = (order.Note ?? "") + $"\n[VNPay] Thanh toán thất bại - Mã lỗi: {response.ResponseCode}";

                _logger.LogWarning("VNPay: Thanh toán thất bại cho đơn {OrderCode}, Mã lỗi: {ResponseCode}",
                    order.OrderCode, response.ResponseCode);

                TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {response.ResponseCode}. Vui lòng thử lại hoặc chọn phương thức khác.";
            }

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("OrderConfirmation", "Tour", new { orderId = order.Id });
        }
    }
}
