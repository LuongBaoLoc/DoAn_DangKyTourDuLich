namespace DoAn_DangKyTourDuLich.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ thanh toán VNPay
    /// </summary>
    public interface IVnPayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
        string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress);

        /// <summary>
        /// Xác thực response từ VNPay callback
        /// </summary>
        VnPayResponseModel ProcessPaymentResponse(IQueryCollection queryParams);
    }

    public class VnPayResponseModel
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
