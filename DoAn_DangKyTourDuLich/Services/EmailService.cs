using MailKit.Net.Smtp;
using MimeKit;

namespace DoAn_DangKyTourDuLich.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendBookingEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal totalAmount)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Hệ Thống Tour Lộc", emailSettings["Email"]));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"[XÁC NHẬN ĐƠN HÀNG] Mã đơn: {orderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50; text-align: center;'>ĐẶT TOUR THÀNH CÔNG!</h2>
                    <p>Chào <b>{customerName}</b>,</p>
                    <p>Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi. Dưới đây là thông tin đơn hàng của bạn:</p>
                    <div style='background: #f9f9f9; padding: 15px; border-radius: 5px;'>
                        <p><b>Mã đơn hàng:</b> {orderCode}</p>
                        <p><b>Tour đã chọn:</b> {tourName}</p>
                        <p><b>Tổng cộng:</b> <span style='color: #e74c3c; font-weight: bold;'>{totalAmount.ToString("N0")} VNĐ</span></p>
                    </div>
                    <p>Lịch trình chi tiết và hóa đơn điện tử sẽ được gửi kèm trong email tiếp theo sau khi chúng tôi xác nhận thanh toán.</p>
                    <p>Nếu có thắc mắc, vui lòng liên hệ Hotline: 0123.456.789</p>
                    <hr>
                    <p style='font-size: 12px; color: #888; text-align: center;'>Đây là email tự động, vui lòng không phản hồi email này.</p>
                </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["Email"], emailSettings["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Có thể ghi log lỗi ở đây nếu gửi thất bại
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }
        }

        public async Task SendAdminNotificationEmailAsync(string customerName, string customerPhone, string tourName, string orderCode, decimal totalAmount)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var adminEmail = emailSettings["Email"]; // Gửi thẳng vào email của admin (được cấu hình trong appsettings)
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hệ Thống Bot", adminEmail));
            message.To.Add(new MailboxAddress("Admin", adminEmail));
            message.Subject = $"[ĐH MỚI] Khách hàng {customerName} vừa đặt tour - Mã: {orderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #e74c3c; text-align: center;'>CÓ ĐƠN ĐẶT TOUR MỚI!</h2>
                    <div style='background: #f9f9f9; padding: 15px; border-radius: 5px;'>
                        <p><b>Mã đơn hàng:</b> {orderCode}</p>
                        <p><b>Tên khách hàng:</b> {customerName}</p>
                        <p><b>Số điện thoại:</b> {customerPhone}</p>
                        <p><b>Tour đã chọn:</b> {tourName}</p>
                        <p><b>Tổng cộng:</b> <span style='color: #e74c3c; font-weight: bold;'>{totalAmount.ToString("N0")} VNĐ</span></p>
                    </div>
                    <p>Vui lòng đăng nhập vào trang quản trị để xem chi tiết và tiến hành xác nhận với khách hàng.</p>
                </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["Email"], emailSettings["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi mail thông báo cho admin: " + ex.Message);
            }
        }
    }
}
