using QRCoder;

namespace DoAn_DangKyTourDuLich.Services
{
    public class QRCodeService
    {
        public byte[] GenerateQRCode(string content, int pixelsPerModule = 10)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
            return qrCodeBytes;
        }

        public string GenerateQRCodeBase64(string content, int pixelsPerModule = 10)
        {
            var qrCodeBytes = GenerateQRCode(content, pixelsPerModule);
            return Convert.ToBase64String(qrCodeBytes);
        }

        public string GetBookingTicketInfo(string orderCode, string customerName, string tourName, DateTime orderDate, decimal totalAmount)
        {
            return $"Mã ĐH: {orderCode}\n" +
                   $"Khách: {customerName}\n" +
                   $"Tour: {tourName}\n" +
                   $"Ngày: {orderDate:dd/MM/yyyy}\n" +
                   $"Giá: {totalAmount:N0} VNĐ";
        }
    }
}
