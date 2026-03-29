using MailKit.Net.Smtp;
using MimeKit;

namespace DoAn_DangKyTourDuLich.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly QRCodeService _qrCodeService;

        public EmailService(IConfiguration config, QRCodeService qrCodeService)
        {
            _config = config;
            _qrCodeService = qrCodeService;
        }

        public async Task SendBookingEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal totalAmount, DateTime orderDate)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Hệ Thống Tour Du Lich", emailSettings["Email"]));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"[XÁC NHẬN ĐƠN HÀNG] Mã đơn: {orderCode}";

            var bodyBuilder = new BodyBuilder();
            
            // Tạo mã QR - trả về bytes
            var ticketInfo = _qrCodeService.GetBookingTicketInfo(orderCode, customerName, tourName, orderDate, totalAmount);
            var qrCodeBytes = _qrCodeService.GenerateQRCode(ticketInfo);
            
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50; text-align: center;'>ĐẶT TOUR THÀNH CÔNG!</h2>
                    <p>Chào <b>{customerName}</b>,</p>
                    <p>Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi. Dưới đây là thông tin đơn hàng của bạn:</p>
                    <div style='background: #f9f9f9; padding: 15px; border-radius: 5px;'>
                        <p><b>Mã đơn hàng:</b> {orderCode}</p>
                        <p><b>Tour đã chọn:</b> {tourName}</p>
                        <p><b>Ngày đặt:</b> {orderDate:dd/MM/yyyy HH:mm}</p>
                        <p><b>Tổng cộng:</b> <span style='color: #e74c3c; font-weight: bold;'>{totalAmount.ToString("N0")} VNĐ</span></p>
                    </div>
                    <div style='text-align: center; margin: 20px 0; padding: 15px; background: #f0f0f0; border-radius: 5px;'>
                        <p style='color: #666; margin-bottom: 10px;'><b>Mã vé của bạn:</b></p>
                        <img src='cid:qrcode' alt='QR Code - Thông tin vé' style='width: 200px; height: 200px; border: 2px solid #ddd; border-radius: 4px; padding: 5px; background: white;' />
                        <p style='margin-top: 10px; font-size: 12px; color: #999;'>Vui lòng giữ kĩ QR code này, mang theo khi tham gia tour</p>
                    </div>
                    <p>Nếu có thắc mắc, vui lòng liên hệ Hotline: 0385353174</p>
                    <hr>
                    <p style='font-size: 12px; color: #888; text-align: center;'>Đây là email tự động, vui lòng không phản hồi email này.</p>
                </div>";

            // Gắn QR code vào email như attachment inline
            bodyBuilder.LinkedResources.Add("qrcode", new MemoryStream(qrCodeBytes), new MimeKit.ContentType("image", "png"));

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

        public async Task SendRefundEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal refundAmount)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Hệ Thống Tour Lộc", emailSettings["Email"]));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"[HOÀN TIỀN] Đơn hàng {orderCode} - Tour: {tourName}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #e74c3c; text-align: center;'>THÔNG BÁO HỦY CHUYẾN ĐI</h2>
                    <p>Chào <b>{customerName}</b>,</p>
                    <p>Rất tiếc, chúng tôi phải thông báo rằng chuyến tour <b>{tourName}</b> (Mã: <b>{orderCode}</b>) 
                    phải bị hủy vì <b>không đủ số lượng khách hàng</b> để xuất phát.</p>
                    
                    <div style='background: #fff3cd; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #ffc107;'>
                        <p style='margin: 0; color: #856404;'>
                            <b>💰 Thông tin hoàn tiền:</b><br/>
                            Số tiền sẽ được hoàn lại: <span style='color: #e74c3c; font-weight: bold;'>{refundAmount.ToString("N0")} VNĐ</span><br/>
                            Thời gian hoàn tiền: 3-5 ngày làm việc vào tài khoản bạn đã cung cấp.
                        </p>
                    </div>
                    
                    <p>Chúng tôi xin lỗi vì sự bất tiện này. Nếu bạn quan tâm, chúng tôi sẽ sắp xếp các chuyến tour khác tương tự.</p>
                    
                    <p>Để tìm hiểu thêm về các tour khác hoặc nhận hỗ trợ, vui lòng liên hệ:</p>
                    <p style='text-align: center; margin: 20px 0;'>
                        <b>Hotline: 0385353174</b><br/>
                        <b>Email: luongbaoloc2014@gmail.com</b>
                    </p>
                    
                    <p>Cảm ơn bạn đã hiểu biết và ủng hộ chúng tôi!</p>
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
                Console.WriteLine("Lỗi gửi mail hoàn tiền: " + ex.Message);
            }
        }
    }
}
