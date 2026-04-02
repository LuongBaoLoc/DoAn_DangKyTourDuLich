using DoAn_DangKyTourDuLich.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace DoAn_DangKyTourDuLich.Services
{
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
        public DateTime? SelectedDepartureDateTime { get; set; }
        public string? GroupTypeDisplay { get; set; }
        public string OrderCode { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ConfirmedAt { get; set; }
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

        private static string BuildRow(string label, string value, string color = "#333")
        {
            return $@"
            <tr>
                <td style='padding:8px 12px;color:#777;white-space:nowrap;vertical-align:top;'>{label}</td>
                <td style='padding:8px 12px;color:{color};font-weight:600;'>{value}</td>
            </tr>";
        }

        private static string WrapCard(string title, string accent, string rows)
        {
            return $@"
            <div style='background:#f8f9fa;border:1px solid #e9ecef;border-radius:12px;padding:4px;margin-bottom:18px;'>
                <div style='background:{accent};color:#fff;padding:10px 16px;border-radius:8px 8px 0 0;font-weight:700;text-transform:uppercase;font-size:13px;'>{title}</div>
                <table width='100%' style='border-collapse:collapse;background:#fff;border-radius:0 0 8px 8px;'>{rows}</table>
            </div>";
        }

        private static string EmailHeader() => @"
            <div style='background:linear-gradient(135deg,#1a73e8 0%,#00c9a7 100%);padding:28px 20px;text-align:center;'>
                <div style='font-size:28px;font-weight:800;color:#fff;'>Tour Du Lich</div>
                <div style='font-size:13px;color:rgba(255,255,255,.85);margin-top:4px;'>Xac nhan va dong hanh cung ban</div>
            </div>";

        private static string EmailFooter() => @"
            <div style='background:#f8f9fa;padding:22px 28px;border-top:1px solid #eee;'>
                <div style='font-size:13px;color:#666;text-align:center;'>Hotline: <b>0385 353 174</b> | Email: <b>luongbaoloc2014@gmail.com</b></div>
                <p style='font-size:11px;color:#999;text-align:center;margin:12px 0 0;'>Email tu dong tu he thong. Vui long khong phan hoi email nay.</p>
            </div>";

        public async Task SendBookingEmailAsync(BookingEmailInfo info)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Tour Du Lich", emailSettings["Email"] ?? ""));
            message.To.Add(new MailboxAddress(info.CustomerName, info.CustomerEmail));
            message.Subject = $"Xac nhan chuyen tour - Ma don {info.OrderCode}";

            var bodyBuilder = new BodyBuilder();
            var ticket = _qrCodeService.GetBookingTicketInfo(info.OrderCode, info.CustomerName, info.TourName, info.OrderDate, info.TotalAmount);
            var qrCodeBytes = _qrCodeService.GenerateQRCode(ticket);

            DateTime? actualDeparture = info.SelectedDepartureDateTime ?? info.TourDepartureDate;
            string departureDate = actualDeparture?.ToString("dd/MM/yyyy") ?? "Lien he";
            string departureTime = actualDeparture?.ToString("HH:mm") ?? "Lien he";
            string returnDate = actualDeparture.HasValue
                ? actualDeparture.Value.AddDays(Math.Max(info.TourDuration - 1, 0)).ToString("dd/MM/yyyy")
                : "Lien he";

            string orderRows =
                BuildRow("Ma don:", info.OrderCode, "#1a73e8") +
                BuildRow("Ngay dat:", info.OrderDate.ToString("dd/MM/yyyy HH:mm")) +
                BuildRow("Xac nhan luc:", info.ConfirmedAt?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm")) +
                BuildRow("Thanh toan:", info.PaymentMethodDisplay);

            string tripRows =
                BuildRow("Tour:", info.TourName) +
                BuildRow("Loai chuyen:", info.GroupTypeDisplay ?? "Tour ghep") +
                BuildRow("Khoi hanh tu:", info.TourDepartureLocation ?? "N/A") +
                BuildRow("Diem den:", info.TourDestination ?? "N/A") +
                BuildRow("Ngay di:", departureDate) +
                BuildRow("Gio khoi hanh:", departureTime) +
                BuildRow("Ngay ve du kien:", returnDate) +
                BuildRow("Thoi gian:", $"{info.TourDuration} ngay") +
                BuildRow("Phuong tien:", info.TourTransportation ?? "Xe du lich");

            string customerRows =
                BuildRow("Ho ten:", info.CustomerName) +
                BuildRow("Dien thoai:", info.CustomerPhone) +
                BuildRow("Email:", info.CustomerEmail) +
                BuildRow("Dia chi:", info.CustomerAddress ?? "N/A");

            string priceRows = "";
            if (info.AdultQuantity > 0)
            {
                priceRows += $@"
                <tr>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;'>Nguoi lon</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:center;'>{info.AdultQuantity}</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:right;'>{info.AdultPrice:N0} d</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:right;font-weight:600;'>{(info.AdultQuantity * info.AdultPrice):N0} d</td>
                </tr>";
            }

            if (info.ChildQuantity > 0)
            {
                priceRows += $@"
                <tr>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;'>Tre em (5-11)</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:center;'>{info.ChildQuantity}</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:right;'>{info.ChildPrice:N0} d</td>
                    <td style='padding:10px 12px;border-bottom:1px solid #f0f0f0;text-align:right;font-weight:600;'>{(info.ChildQuantity * info.ChildPrice):N0} d</td>
                </tr>";
            }

            string pricingTable = $@"
            <div style='background:#f8f9fa;border:1px solid #e9ecef;border-radius:12px;padding:4px;margin-bottom:18px;'>
                <div style='background:#e02000;color:#fff;padding:10px 16px;border-radius:8px 8px 0 0;font-weight:700;text-transform:uppercase;font-size:13px;'>Chi tiet gia</div>
                <table width='100%' style='border-collapse:collapse;background:#fff;font-size:13px;'>
                    <tr>
                        <th style='padding:10px 12px;text-align:left;border-bottom:2px solid #e9ecef;'>Hang</th>
                        <th style='padding:10px 12px;text-align:center;border-bottom:2px solid #e9ecef;'>SL</th>
                        <th style='padding:10px 12px;text-align:right;border-bottom:2px solid #e9ecef;'>Don gia</th>
                        <th style='padding:10px 12px;text-align:right;border-bottom:2px solid #e9ecef;'>Thanh tien</th>
                    </tr>
                    {priceRows}
                    <tr style='background:#fff8f8;'>
                        <td colspan='3' style='padding:12px;text-align:right;font-weight:700;'>Tong cong:</td>
                        <td style='padding:12px;text-align:right;font-size:18px;font-weight:800;color:#e02000;'>{info.TotalAmount:N0} d</td>
                    </tr>
                </table>
            </div>";

            string noteBlock = string.IsNullOrWhiteSpace(info.Note)
                ? ""
                : $@"
                <div style='background:#fff3cd;border:1px solid #ffc107;border-radius:10px;padding:14px 16px;margin-bottom:18px;font-size:13px;line-height:1.6;'>
                    <b>Ghi chu:</b><br>{info.Note.Replace("\n", "<br>")}
                </div>";

            bodyBuilder.HtmlBody = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:680px;margin:20px auto;background:#fff;border-radius:14px;box-shadow:0 4px 24px rgba(0,0,0,.08);overflow:hidden;'>
                {EmailHeader()}
                <div style='padding:28px 30px;'>
                    <div style='background:linear-gradient(135deg,#d4edda,#c3e6cb);border:1px solid #b1dfbb;border-radius:12px;padding:20px;text-align:center;margin-bottom:22px;'>
                        <div style='font-size:18px;font-weight:800;color:#155724;'>Admin da xac nhan chuyen tour cua ban</div>
                        <div style='margin-top:6px;font-size:13px;color:#155724;'>Thong tin chuyen di chi tiet duoc gui o ben duoi</div>
                    </div>

                    <p style='font-size:14px;color:#555;line-height:1.7;'>
                        Chao <b>{info.CustomerName}</b>, don tour cua ban da duoc admin xac nhan thanh cong. Vui long kiem tra ky thong tin ngay di, gio khoi hanh va ma don de san sang cho chuyen di.
                    </p>

                    {WrapCard("Thong tin don hang", "#1a73e8", orderRows)}
                    {WrapCard("Thong tin chuyen di", "#00c9a7", tripRows)}
                    {WrapCard("Thong tin khach hang", "#6f42c1", customerRows)}
                    {pricingTable}
                    {noteBlock}

                    <div style='text-align:center;margin:24px 0;padding:20px;background:linear-gradient(135deg,#f8f9fa,#e9ecef);border-radius:12px;border:1px dashed #ccc;'>
                        <div style='font-weight:700;color:#333;margin-bottom:12px;'>Ma QR xac nhan chuyen di</div>
                        <img src='cid:qrcode' alt='QR Code' style='width:180px;height:180px;border:3px solid #fff;border-radius:12px;box-shadow:0 4px 12px rgba(0,0,0,.1);background:#fff;padding:8px;' />
                        <div style='margin-top:10px;font-size:12px;color:#777;'>Vui long mang theo ma QR khi tham gia tour</div>
                    </div>

                    <div style='background:#e8f4fd;border-left:4px solid #1a73e8;border-radius:0 8px 8px 0;padding:14px 16px;font-size:12px;color:#555;line-height:1.8;'>
                        <b>Luu y:</b><br>
                        - Co mat truoc gio khoi hanh it nhat 30 phut.<br>
                        - Mang theo CCCD/CMND hoac giay to tuy than hop le.<br>
                        - Neu can ho tro them, vui long lien he hotline cua chung toi.
                    </div>
                </div>
                {EmailFooter()}
            </div>";

            var qrImage = bodyBuilder.LinkedResources.Add("qrcode.png", new MemoryStream(qrCodeBytes), new ContentType("image", "png"));
            qrImage.ContentId = "qrcode";
            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);
        }

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

        public async Task SendAdminNotificationEmailAsync(BookingEmailInfo info)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var adminEmail = emailSettings["Email"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("He thong Bot", adminEmail ?? ""));
            message.To.Add(new MailboxAddress("Admin", adminEmail ?? ""));
            message.Subject = $"[DON MOI] {info.CustomerName} dat tour - Ma {info.OrderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:640px;margin:20px auto;background:#fff;border-radius:12px;box-shadow:0 4px 24px rgba(0,0,0,.08);overflow:hidden;'>
                <div style='background:linear-gradient(135deg,#e74c3c,#c0392b);padding:24px 20px;text-align:center;color:#fff;font-weight:800;'>DON DAT TOUR MOI</div>
                <div style='padding:24px 30px;'>
                    {WrapCard("Chi tiet don", "#343a40",
                        BuildRow("Ma don:", info.OrderCode, "#e74c3c") +
                        BuildRow("Khach hang:", info.CustomerName) +
                        BuildRow("Dien thoai:", info.CustomerPhone) +
                        BuildRow("Email:", info.CustomerEmail) +
                        BuildRow("Tour:", info.TourName) +
                        BuildRow("Tong tien:", $"{info.TotalAmount:N0} d", "#e74c3c"))}
                </div>
            </div>";
            message.Body = bodyBuilder.ToMessageBody();
            await SendEmailAsync(message);
        }

        public async Task SendRefundEmailAsync(string customerEmail, string customerName, string tourName, string orderCode, decimal refundAmount)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Tour Du Lich", emailSettings["Email"] ?? ""));
            message.To.Add(new MailboxAddress(customerName, customerEmail));
            message.Subject = $"Thong bao huy tour - Ma don {orderCode}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:640px;margin:20px auto;background:#fff;border-radius:12px;box-shadow:0 4px 24px rgba(0,0,0,.08);overflow:hidden;'>
                {EmailHeader()}
                <div style='padding:28px 30px;'>
                    <div style='background:linear-gradient(135deg,#fff3cd,#ffeeba);border:1px solid #ffc107;border-radius:12px;padding:20px;text-align:center;margin-bottom:20px;'>
                        <div style='font-size:18px;font-weight:800;color:#856404;'>Thong bao huy chuyen di</div>
                    </div>
                    {WrapCard("Thong tin hoan tien", "#e02000",
                        BuildRow("Ma don:", orderCode) +
                        BuildRow("Tour:", tourName) +
                        BuildRow("So tien hoan:", $"{refundAmount:N0} d", "#e02000") +
                        BuildRow("Thoi gian hoan:", "3 - 5 ngay lam viec"))}
                </div>
                {EmailFooter()}
            </div>";

            message.Body = bodyBuilder.ToMessageBody();
            await SendEmailAsync(message);
        }

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
                Console.WriteLine("Loi gui mail: " + ex.Message);
            }
        }
    }
}
