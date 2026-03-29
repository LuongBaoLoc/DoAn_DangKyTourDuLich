using MailKit.Net.Smtp;
using MimeKit;
using DoAn_DangKyTourDuLich.Models;

namespace DoAn_DangKyTourDuLich.Services
{
    // DTO để truyền thông tin chi tiết cho email
    public class BookingEmailInfo
    {
        public string CustomerEmail { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string? CustomerAddress { get; set; }
        public string TourName { get; set; } = "";
        public string? TourDestination { get; set; }
        public string? TourDepartureLocation { get; set; }
        public string? TourTransportation { get; set; }
        public int TourDuration { get; set; }
        public DateTime? TourDepartureDate { get; set; }
        public string OrderCode { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int AdultQuantity { get; set; }
        public int ChildQuantity { get; set; }
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public string PaymentMethodDisplay { get; set; } = "";
        public string? Note { get; set; }
    }

    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly QRCodeService _qrCodeService;

        public EmailService(IConfiguration config, QRCodeService qrCodeService)
        {
            _config = config;
            _qrCodeService = qrCodeService;
        }

        private string GetEmailHeader()
        {
            return @"
            <div style='background: linear-gradient(135deg, #1a73e8 0%, #00c9a7 100%); padding: 30px 20px; text-align: center; border-radius: 12px 12px 0 0;'>
                <div style='font-size: 28px; font-weight: 800; color: #fff; letter-spacing: -0.5px;'>
                    🌍 Tour Du Lịch
                </div>
                <div style='font-size: 13px; color: rgba(255,255,255,0.85); margin-top: 4px;'>Khám phá Việt Nam cùng chúng tôi</div>
            </div>";
        }

        private string GetEmailFooter()
        {
            return @"
            <div style='background: #f8f9fa; padding: 24px 30px; border-radius: 0 0 12px 12px; border-top: 1px solid #eee;'>
                <table width='100%' style='font-size: 13px; color: #666;'>
                    <tr>
                        <td style='text-align: center; padding: 4px 0;'>📞 Hotline: <b style='color:#333;'>0385 353 174</b></td>
                    </tr>
                    <tr>
                        <td style='text-align: center; padding: 4px 0;'>📧 Email: <b style='color:#333;'>luongbaoloc2014@gmail.com</b></td>
                    </tr>
                    <tr>
                        <td style='text-align: center; padding: 4px 0;'>⏰ Hỗ trợ: <b style='color:#333;'>08:00 - 22:00 hàng ngày</b></td>
                    </tr>
                </table>
                <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 16px 0;'>
                <p style='font-size: 11px; color: #aaa; text-align: center; margin: 0;'>
                    Đây là email tự động từ hệ thống Tour Du Lịch. Vui lòng không phản hồi email này.
                </p>
            </div>";
        }

        private string GetInfoRow(string label, string value, string valueColor = "#333")
        {
            return $@"
            <tr>
                <td style='padding: 8px 12px; color: #888; font-size: 13px; white-space: nowrap; vertical-align: top;'>{label}</td>
                <td style='padding: 8px 12px; font-weight: 600; color: {valueColor}; font-size: 13px;'>{value}</td>
            </tr>";
        }

        // ====== EMAIL XÁC NHẬN ĐƠN HÀNG (gửi cho khách) ======
        public async Task SendBookingEmailAsync(BookingEmailInfo info)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Tour Du Lịch", emailSettings["Email"] ?? ""));
            message.To.Add(new MailboxAddress(info.CustomerName, info.CustomerEmail));
            message.Subject = $"✅ Xác nhận đặt tour thành công - Mã đơn: {info.OrderCode}";

            var bodyBuilder = new BodyBuilder();

            // QR Code
            var ticketStr = _qrCodeService.GetBookingTicketInfo(info.OrderCode, info.CustomerName, info.TourName, info.OrderDate, info.TotalAmount);
            var qrCodeBytes = _qrCodeService.GenerateQRCode(ticketStr);

            // Tính ngày về
            string departureStr = info.TourDepartureDate?.ToString("dd/MM/yyyy") ?? "Liên hệ";
            string returnStr = info.TourDepartureDate.HasValue
                ? info.TourDepartureDate.Value.AddDays(info.TourDuration - 1).ToString("dd/MM/yyyy")
                : "Liên hệ";

            // Build bảng chi tiết giá
            string priceRows = "";
            if (info.AdultQuantity > 0)
                priceRows += $@"
                <tr>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0;'>Người lớn (từ 12 tuổi)</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: center;'>{info.AdultQuantity}</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: right;'>{info.AdultPrice:N0} đ</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: right; font-weight: 600;'>{(info.AdultQuantity * info.AdultPrice):N0} đ</td>
                </tr>";
            if (info.ChildQuantity > 0)
                priceRows += $@"
                <tr>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0;'>Trẻ nhỏ (5-11 tuổi)</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: center;'>{info.ChildQuantity}</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: right;'>{info.ChildPrice:N0} đ</td>
                    <td style='padding: 10px 12px; border-bottom: 1px solid #f0f0f0; text-align: right; font-weight: 600;'>{(info.ChildQuantity * info.ChildPrice):N0} đ</td>
                </tr>";

            bodyBuilder.HtmlBody = $@"
            <div style='font-family: ""Segoe UI"", Arial, sans-serif; max-width: 640px; margin: 20px auto; background: #fff; border-radius: 12px; box-shadow: 0 4px 24px rgba(0,0,0,0.08); overflow: hidden;'>
                {GetEmailHeader()}

                <div style='padding: 30px;'>
                    <!-- Banner thành công -->
                    <div style='background: linear-gradient(135deg, #d4edda, #c3e6cb); border: 1px solid #b1dfbb; border-radius: 10px; padding: 20px; text-align: center; margin-bottom: 24px;'>
                        <div style='font-size: 36px; margin-bottom: 8px;'>🎉</div>
                        <div style='font-size: 18px; font-weight: 700; color: #155724;'>ĐẶT TOUR THÀNH CÔNG!</div>
                        <div style='font-size: 13px; color: #155724; margin-top: 4px;'>Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi</div>
                    </div>

                    <p style='font-size: 14px; color: #555; margin-bottom: 20px;'>
                        Chào <b style='color:#333;'>{info.CustomerName}</b>, đơn hàng của bạn đã được ghi nhận. Dưới đây là thông tin chi tiết:
                    </p>

                    <!-- THÔNG TIN ĐƠN HÀNG -->
                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 20px; border: 1px solid #e9ecef;'>
                        <div style='background: #1a73e8; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;'>
                            📋 Thông tin đơn hàng
                        </div>
                        <table width='100%' style='border-collapse: collapse;'>
                            {GetInfoRow("Mã đơn hàng:", info.OrderCode, "#1a73e8")}
                            {GetInfoRow("Ngày đặt:", info.OrderDate.ToString("dd/MM/yyyy HH:mm"))}
                            {GetInfoRow("Thanh toán:", info.PaymentMethodDisplay)}
                        </table>
                    </div>

                    <!-- THÔNG TIN TOUR -->
                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 20px; border: 1px solid #e9ecef;'>
                        <div style='background: #00c9a7; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;'>
                            🗺️ Thông tin chuyến đi
                        </div>
                        <table width='100%' style='border-collapse: collapse;'>
                            {GetInfoRow("Tour:", info.TourName)}
                            {GetInfoRow("Điểm khởi hành:", info.TourDepartureLocation ?? "N/A")}
                            {GetInfoRow("Điểm đến:", info.TourDestination ?? "N/A")}
                            {GetInfoRow("Thời gian:", $"{info.TourDuration} ngày")}
                            {GetInfoRow("Ngày đi:", departureStr)}
                            {GetInfoRow("Ngày về (dự kiến):", returnStr)}
                            {GetInfoRow("Phương tiện:", info.TourTransportation ?? "Xe du lịch")}
                        </table>
                    </div>

                    <!-- THÔNG TIN KHÁCH HÀNG -->
                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 20px; border: 1px solid #e9ecef;'>
                        <div style='background: #6f42c1; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;'>
                            👤 Thông tin khách hàng
                        </div>
                        <table width='100%' style='border-collapse: collapse;'>
                            {GetInfoRow("Họ tên:", info.CustomerName)}
                            {GetInfoRow("Điện thoại:", info.CustomerPhone)}
                            {GetInfoRow("Email:", info.CustomerEmail)}
                            {GetInfoRow("Địa chỉ:", info.CustomerAddress ?? "N/A")}
                        </table>
                    </div>

                    <!-- BẢNG GIÁ CHI TIẾT -->
                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 20px; border: 1px solid #e9ecef;'>
                        <div style='background: #e02000; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;'>
                            💰 Chi tiết giá
                        </div>
                        <table width='100%' style='border-collapse: collapse; font-size: 13px;'>
                            <tr style='background: #fff;'>
                                <th style='padding: 10px 12px; text-align: left; color: #666; font-weight: 600; border-bottom: 2px solid #e9ecef;'>Hạng</th>
                                <th style='padding: 10px 12px; text-align: center; color: #666; font-weight: 600; border-bottom: 2px solid #e9ecef;'>SL</th>
                                <th style='padding: 10px 12px; text-align: right; color: #666; font-weight: 600; border-bottom: 2px solid #e9ecef;'>Đơn giá</th>
                                <th style='padding: 10px 12px; text-align: right; color: #666; font-weight: 600; border-bottom: 2px solid #e9ecef;'>Thành tiền</th>
                            </tr>
                            {priceRows}
                            <tr style='background: #fff8f8;'>
                                <td colspan='3' style='padding: 12px; text-align: right; font-weight: 700; font-size: 15px; color: #333;'>TỔNG CỘNG:</td>
                                <td style='padding: 12px; text-align: right; font-weight: 800; font-size: 18px; color: #e02000;'>{info.TotalAmount:N0} đ</td>
                            </tr>
                        </table>
                    </div>

                    {(!string.IsNullOrEmpty(info.Note) ? $@"
                    <div style='background: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 12px 16px; margin-bottom: 20px; font-size: 13px;'>
                        <b>📝 Ghi chú:</b> {info.Note}
                    </div>" : "")}

                    <!-- QR CODE -->
                    <div style='text-align: center; margin: 24px 0; padding: 20px; background: linear-gradient(135deg, #f8f9fa, #e9ecef); border-radius: 12px; border: 1px dashed #ccc;'>
                        <div style='font-weight: 700; color: #333; margin-bottom: 12px; font-size: 14px;'>🎫 MÃ VÉ ĐIỆN TỬ CỦA BẠN</div>
                        <img src='cid:qrcode' alt='QR Code' style='width: 180px; height: 180px; border: 3px solid #fff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); background: white; padding: 8px;' />
                        <div style='margin-top: 10px; font-size: 12px; color: #888;'>Vui lòng mang theo mã QR này khi tham gia tour</div>
                    </div>

                    <!-- LƯU Ý -->
                    <div style='background: #e8f4fd; border-left: 4px solid #1a73e8; border-radius: 0 8px 8px 0; padding: 14px 16px; margin-bottom: 16px; font-size: 12px; color: #555;'>
                        <b style='color: #1a73e8;'>📌 Lưu ý quan trọng:</b><br>
                        • Vui lòng có mặt tại điểm tập trung trước giờ khởi hành 30 phút.<br>
                        • Mang theo CMND/CCCD bản gốc khi tham gia tour.<br>
                        • Hủy tour trước 7 ngày sẽ được hoàn 100% chi phí.
                    </div>
                </div>

                {GetEmailFooter()}
            </div>";

            var qrImage = bodyBuilder.LinkedResources.Add("qrcode.png", new MemoryStream(qrCodeBytes), new MimeKit.ContentType("image", "png"));
            qrImage.ContentId = "qrcode";
            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

        // ====== Overload cũ để tương thích (nếu cần) ======
        public async Task SendBookingEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal totalAmount, DateTime orderDate)
        {
            await SendBookingEmailAsync(new BookingEmailInfo
            {
                CustomerEmail = customerEmail,
                CustomerName = customerName,
                TourName = tourName,
                OrderCode = orderCode,
                TotalAmount = totalAmount,
                OrderDate = orderDate,
                AdultQuantity = 1,
                AdultPrice = totalAmount,
                PaymentMethodDisplay = "N/A"
            });
        }

        // ====== EMAIL THÔNG BÁO CHO ADMIN ======
        public async Task SendAdminNotificationEmailAsync(BookingEmailInfo info)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var adminEmail = emailSettings["Email"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hệ Thống Bot", adminEmail ?? ""));
            message.To.Add(new MailboxAddress("Admin", adminEmail ?? ""));
            message.Subject = $"🔔 [ĐƠN MỚI] {info.CustomerName} đặt tour - Mã: {info.OrderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: ""Segoe UI"", Arial, sans-serif; max-width: 640px; margin: 20px auto; background: #fff; border-radius: 12px; box-shadow: 0 4px 24px rgba(0,0,0,0.08); overflow: hidden;'>
                <!-- Header Admin -->
                <div style='background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%); padding: 24px 20px; text-align: center; border-radius: 12px 12px 0 0;'>
                    <div style='font-size: 24px; font-weight: 800; color: #fff;'>🔔 ĐƠN ĐẶT TOUR MỚI!</div>
                    <div style='font-size: 12px; color: rgba(255,255,255,0.8); margin-top: 4px;'>Thông báo tự động từ hệ thống</div>
                </div>

                <div style='padding: 24px 30px;'>
                    <div style='background: #fff3cd; border: 1px solid #ffc107; border-radius: 10px; padding: 16px; text-align: center; margin-bottom: 20px;'>
                        <div style='font-size: 14px; color: #856404; font-weight: 600;'>⚡ Vui lòng xác nhận đơn hàng sớm nhất có thể</div>
                    </div>

                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 16px; border: 1px solid #e9ecef;'>
                        <div style='background: #343a40; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase;'>
                            📋 Chi tiết đơn hàng
                        </div>
                        <table width='100%' style='border-collapse: collapse;'>
                            {GetInfoRow("Mã đơn:", info.OrderCode, "#e74c3c")}
                            {GetInfoRow("Khách hàng:", info.CustomerName)}
                            {GetInfoRow("SĐT:", info.CustomerPhone)}
                            {GetInfoRow("Email:", info.CustomerEmail)}
                            {GetInfoRow("Tour:", info.TourName)}
                            {GetInfoRow("Số lượng:", $"{info.AdultQuantity} Lớn, {info.ChildQuantity} Trẻ nhỏ")}
                            {GetInfoRow("Thanh toán:", info.PaymentMethodDisplay)}
                            {GetInfoRow("Tổng tiền:", $"{info.TotalAmount:N0} VNĐ", "#e74c3c")}
                        </table>
                    </div>

                    <div style='text-align: center; padding: 16px 0;'>
                        <span style='display: inline-block; background: linear-gradient(135deg, #1a73e8, #1557b0); color: white; padding: 12px 32px; border-radius: 8px; font-weight: 700; font-size: 14px;'>
                            👉 Đăng nhập Admin để xác nhận
                        </span>
                    </div>
                </div>

                <div style='background: #f8f9fa; padding: 16px 30px; border-radius: 0 0 12px 12px; border-top: 1px solid #eee;'>
                    <p style='font-size: 11px; color: #aaa; text-align: center; margin: 0;'>Email tự động - Hệ thống quản lý Tour Du Lịch</p>
                </div>
            </div>";

            message.Body = bodyBuilder.ToMessageBody();
            await SendEmailAsync(message);
        }

        // ====== EMAIL HOÀN TIỀN / HỦY TOUR (gửi cho khách) ======
        public async Task SendRefundEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal refundAmount)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Tour Du Lịch", emailSettings["Email"] ?? ""));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"⚠️ Thông báo hủy tour - Mã đơn: {orderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: ""Segoe UI"", Arial, sans-serif; max-width: 640px; margin: 20px auto; background: #fff; border-radius: 12px; box-shadow: 0 4px 24px rgba(0,0,0,0.08); overflow: hidden;'>
                {GetEmailHeader()}

                <div style='padding: 30px;'>
                    <!-- Banner thông báo -->
                    <div style='background: linear-gradient(135deg, #fff3cd, #ffeeba); border: 1px solid #ffc107; border-radius: 10px; padding: 20px; text-align: center; margin-bottom: 24px;'>
                        <div style='font-size: 36px; margin-bottom: 8px;'>😔</div>
                        <div style='font-size: 18px; font-weight: 700; color: #856404;'>THÔNG BÁO HỦY CHUYẾN ĐI</div>
                        <div style='font-size: 13px; color: #856404; margin-top: 4px;'>Chúng tôi rất tiếc về sự bất tiện này</div>
                    </div>

                    <p style='font-size: 14px; color: #555; margin-bottom: 20px; line-height: 1.6;'>
                        Chào <b style='color:#333;'>{customerName}</b>,<br><br>
                        Rất tiếc, chúng tôi phải thông báo rằng chuyến tour <b style='color: #1a73e8;'>{tourName}</b> 
                        (Mã: <b>{orderCode}</b>) đã bị hủy.
                    </p>

                    <!-- Thông tin hoàn tiền -->
                    <div style='background: #f8f9fa; border-radius: 10px; padding: 4px; margin-bottom: 20px; border: 1px solid #e9ecef;'>
                        <div style='background: #e02000; color: white; padding: 10px 16px; border-radius: 8px 8px 0 0; font-weight: 700; font-size: 13px; text-transform: uppercase; letter-spacing: 0.5px;'>
                            💰 Thông tin hoàn tiền
                        </div>
                        <table width='100%' style='border-collapse: collapse;'>
                            {GetInfoRow("Mã đơn hàng:", orderCode)}
                            {GetInfoRow("Tour:", tourName)}
                            {GetInfoRow("Số tiền hoàn:", $"{refundAmount:N0} VNĐ", "#e02000")}
                            {GetInfoRow("Thời gian hoàn:", "3 - 5 ngày làm việc")}
                            {GetInfoRow("Hình thức:", "Hoàn vào tài khoản đã cung cấp")}
                        </table>
                    </div>

                    <!-- Gợi ý -->
                    <div style='background: #e8f4fd; border-left: 4px solid #1a73e8; border-radius: 0 8px 8px 0; padding: 14px 16px; margin-bottom: 16px; font-size: 13px; color: #555;'>
                        <b style='color: #1a73e8;'>💡 Gợi ý cho bạn:</b><br>
                        Bạn có thể tham khảo các chuyến tour khác phù hợp tại website của chúng tôi. 
                        Liên hệ Hotline <b>0385 353 174</b> để được tư vấn miễn phí!
                    </div>

                    <p style='font-size: 13px; color: #888; text-align: center;'>Cảm ơn bạn đã thông cảm và ủng hộ chúng tôi! ❤️</p>
                </div>

                {GetEmailFooter()}
            </div>";

            message.Body = bodyBuilder.ToMessageBody();
            await SendEmailAsync(message);
        }

        // ====== Gửi mail helper ======
        private async Task SendEmailAsync(MimeMessage message)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"] ?? "587"), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["Email"], emailSettings["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }
        }
    }
}
