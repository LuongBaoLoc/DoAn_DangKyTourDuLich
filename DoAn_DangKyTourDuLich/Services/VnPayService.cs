using DoAn_DangKyTourDuLich.Services.Interfaces;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DoAn_DangKyTourDuLich.Services
{
    /// <summary>
    /// Dịch vụ tích hợp cổng thanh toán VNPay.
    /// Tạo URL redirect đến VNPay để khách hàng thanh toán,
    /// sau đó xử lý callback response từ VNPay.
    /// 
    /// Tham khảo: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
    /// </summary>
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VnPayService> _logger;

        public VnPayService(IConfiguration config, ILogger<VnPayService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress)
        {
            var vnpay = _config.GetSection("VnPay");
            var vnp_TmnCode = vnpay["TmnCode"] ?? "";
            var vnp_HashSecret = vnpay["HashSecret"] ?? "";
            var vnp_BaseUrl = vnpay["BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var vnp_ReturnUrl = vnpay["ReturnUrl"] ?? "";

            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderId.ToString() },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "travel" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", vnp_ReturnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            // Tạo chuỗi hash
            var signData = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));
            var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);

            var paymentUrl = $"{vnp_BaseUrl}?{signData}&vnp_SecureHash={vnp_SecureHash}";

            _logger.LogInformation("Tạo VNPay URL cho đơn hàng {OrderId}, số tiền: {Amount}", orderId, amount);
            return paymentUrl;
        }

        public VnPayResponseModel ProcessPaymentResponse(IQueryCollection queryParams)
        {
            var vnpay = _config.GetSection("VnPay");
            var vnp_HashSecret = vnpay["HashSecret"] ?? "";

            var response = new VnPayResponseModel();

            if (!queryParams.Any())
            {
                _logger.LogWarning("VNPay callback không có query params");
                return response;
            }

            // Thu thập tất cả params trừ vnp_SecureHash
            var vnp_Params = new SortedDictionary<string, string>();
            string vnp_SecureHash = "";

            foreach (var (key, value) in queryParams)
            {
                if (key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                {
                    vnp_SecureHash = value.ToString();
                    continue;
                }

                if (!string.IsNullOrEmpty(value))
                    vnp_Params[key] = value.ToString();
            }

            // Verify checksum
            var signData = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));
            var checkSum = HmacSHA512(vnp_HashSecret, signData);

            bool isValidSignature = checkSum.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase);

            if (!isValidSignature)
            {
                _logger.LogWarning("VNPay: Chữ ký không hợp lệ cho giao dịch {TxnRef}",
                    vnp_Params.GetValueOrDefault("vnp_TxnRef", "unknown"));
                response.Success = false;
                return response;
            }

            response.OrderId = vnp_Params.GetValueOrDefault("vnp_TxnRef", "");
            response.TransactionId = vnp_Params.GetValueOrDefault("vnp_TransactionNo", "");
            response.ResponseCode = vnp_Params.GetValueOrDefault("vnp_ResponseCode", "");
            response.OrderDescription = vnp_Params.GetValueOrDefault("vnp_OrderInfo", "");
            response.PaymentMethod = vnp_Params.GetValueOrDefault("vnp_BankCode", "");

            if (decimal.TryParse(vnp_Params.GetValueOrDefault("vnp_Amount", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var rawAmount))
                response.Amount = rawAmount / 100;

            response.Success = response.ResponseCode == "00";

            _logger.LogInformation("VNPay callback: OrderId={OrderId}, ResponseCode={ResponseCode}, Success={Success}",
                response.OrderId, response.ResponseCode, response.Success);

            return response;
        }

        /// <summary>
        /// Tạo HMAC SHA512 hash — yêu cầu bởi VNPay API
        /// </summary>
        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
