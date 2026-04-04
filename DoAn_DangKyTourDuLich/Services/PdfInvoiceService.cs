using DoAn_DangKyTourDuLich.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DoAn_DangKyTourDuLich.Services
{
    public class PdfInvoiceService
    {
        private readonly QRCodeService _qrCodeService;

        public PdfInvoiceService(QRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateInvoice(Order order)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    // Lề trang nhìn cân đối hơn
                    page.Margin(2.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(compose => ComposeHeader(compose, order));
                    page.Content().Element(compose => ComposeContent(compose, order));
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Order order)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Trái: Thông tin công ty
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("CÔNG TY DU LỊCH VIỆT NAM").FontSize(14).SemiBold().FontColor("#0d6efd"); // Xanh dương chủ đạo
                        c.Item().Text("Tịnh Biên, An Giang, Việt Nam").FontSize(9).FontColor(Colors.Grey.Medium);
                        c.Item().Text("Hotline: 038 535 3174").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // Phải: Tiêu đề Hóa Đơn
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().AlignRight().Text("HÓA ĐƠN").FontSize(22).SemiBold().FontColor("#dc3545"); // Đỏ nổi bật
                        c.Item().AlignRight().Text($"#{order.OrderCode}").FontSize(10).FontColor("#dc3545");
                    });
                });

                // Đường kẻ ngang màu xanh
                col.Item().PaddingVertical(15).LineHorizontal(2).LineColor("#0d6efd");
            });
        }

        private void ComposeContent(IContainer container, Order order)
        {
            container.PaddingVertical(0).Column(column =>
            {
                column.Spacing(20);
                
                // --- Section 1: Customer & Payment ---
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().PaddingBottom(4).Text("THÔNG TIN KHÁCH HÀNG").FontSize(11).SemiBold().FontColor("#0d6efd");
                        c.Item().Text(t => { t.Span("Họ tên: ").SemiBold(); t.Span(order.CustomerName); });
                        c.Item().Text(t => { t.Span("Email: ").SemiBold(); t.Span(order.CustomerEmail); });
                        c.Item().Text(t => { t.Span("Điện thoại: ").SemiBold(); t.Span(order.CustomerPhone); });
                        c.Item().Text(t => { t.Span("Địa chỉ: ").SemiBold(); t.Span(!string.IsNullOrEmpty(order.CustomerAddress) ? order.CustomerAddress : "Không có"); });
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().AlignRight().PaddingBottom(4).Text("CHI TIẾT THANH TOÁN").FontSize(11).SemiBold().FontColor("#0d6efd");
                        c.Item().AlignRight().Text(t => { t.Span("Ngày đặt: ").SemiBold(); t.Span(order.OrderDate.ToString("dd/MM/yyyy HH:mm")); });
                        
                        string method = order.PaymentMethod == PaymentMethod.OnlinePayment ? "VNPay" : "Tiền mặt";
                        c.Item().AlignRight().Text(t => { t.Span("Phương thức: ").SemiBold(); t.Span(method); });
                        
                        string statusTxt = order.Status switch
                        {
                            OrderStatus.Pending => "Chờ xác nhận",
                            OrderStatus.Confirmed => "Đã xác nhận",
                            OrderStatus.Completed => "Hoàn thành",
                            OrderStatus.Cancelled => "Đã hủy",
                            _ => order.Status.ToString()
                        };
                        c.Item().AlignRight().Text(t => { t.Span("Trạng thái: ").SemiBold(); t.Span(statusTxt); });
                    });
                });

                // --- Section 2: Trip Details ---
                column.Item().Column(c =>
                {
                    c.Item().PaddingBottom(4).Text("CHI TIẾT CHUYẾN ĐI").FontSize(11).SemiBold().FontColor("#0d6efd");
                    
                    var firstDetail = order.OrderDetails.FirstOrDefault();
                    var tour = firstDetail?.Tour;

                    if (tour != null)
                    {
                        c.Item().Text(tour.Name).FontSize(13).SemiBold().FontColor(Colors.Black);
                        c.Item().PaddingBottom(8).Text(t =>
                        {
                            t.Span("Điểm đến: ").SemiBold(); t.Span($"{tour.Destination} | ");
                            t.Span("Thời gian: ").SemiBold(); t.Span($"{tour.Duration} ngày | ");
                            t.Span("Phương tiện: ").SemiBold(); t.Span(tour.Transportation ?? "Xe du lịch");
                        });
                    }

                    // Khung xám Ghi chú
                    c.Item().Background("#f8f9fa").Padding(10).Column(gc =>
                    {
                        gc.Item().Text("Ghi chú:").SemiBold().FontSize(9);
                        gc.Item().Text(string.IsNullOrEmpty(order.Note) ? "Không có ghi chú" : order.Note).FontSize(9);
                    });
                });

                // --- Section 3: Cost Table ---
                column.Item().Column(c =>
                {
                    c.Item().PaddingBottom(4).Text("BẢNG KÊ CHI PHÍ").FontSize(11).SemiBold().FontColor("#0d6efd");
                    c.Item().Element(tableContainer => ComposeTable(tableContainer, order));
                });

                // --- Section 4: Total & Footer ---
                column.Item().PaddingTop(5).Row(row =>
                {
                    // Ghi chú bên trái
                    row.RelativeItem(2).Column(c =>
                    {
                        c.Spacing(3);
                        c.Item().PaddingTop(15).Text("Lưu ý:").FontSize(9).SemiBold();
                        c.Item().Text("• Vui lòng xuất trình mã QR cho hướng dẫn viên khi tham gia tour.").FontSize(9);
                        c.Item().Text("• Mọi thắc mắc xin vui lòng liên hệ Hotline: 038 535 3174.").FontSize(9);
                        c.Item().Text("• Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ!").FontSize(9);
                    });

                    // Tổng tiền bên phải + QR Code
                    row.RelativeItem(1).Column(c =>
                    {
                        // Ô nổi bật màu hồng tổng cộng
                        c.Item().Background("#fce8e8").Padding(15).AlignRight().Column(bc =>
                        {
                            bc.Item().AlignRight().Text("TỔNG CỘNG").FontSize(11).SemiBold().FontColor("#dc3545");
                            bc.Item().AlignRight().Text($"{order.TotalAmount:N0} VNĐ").FontSize(18).SemiBold().FontColor("#dc3545");
                        });

                        // Tạo QR
                        var firstDetail = order.OrderDetails.FirstOrDefault();
                        var tour = firstDetail?.Tour;
                        var qrContent = _qrCodeService.GetBookingTicketInfo(order.OrderCode!, order.CustomerName, tour?.Name ?? "Tour", order.OrderDate, order.TotalAmount);
                        var qrBytes = _qrCodeService.GenerateQRCode(qrContent);

                        c.Item().PaddingTop(15).AlignRight().Column(qrCol =>
                        {
                            qrCol.Item().AlignRight().Width(75).Image(qrBytes);
                            qrCol.Item().AlignRight().PaddingTop(3).Text("E-Ticket QR").FontSize(8).FontColor("#0d6efd").SemiBold();
                        });
                    });
                });
            });
        }

        private void ComposeTable(IContainer container, Order order)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4); // Mô tả
                    columns.RelativeColumn(1); // SL
                    columns.RelativeColumn(2); // Đơn giá
                    columns.RelativeColumn(2); // Thành tiền
                });

                // Dòng Header nền xanh chữ trắng
                table.Header(header =>
                {
                    header.Cell().Background("#0d6efd").Padding(6).Text("Mô tả").FontColor(Colors.White).SemiBold();
                    header.Cell().Background("#0d6efd").Padding(6).AlignCenter().Text("SL").FontColor(Colors.White).SemiBold();
                    header.Cell().Background("#0d6efd").Padding(6).AlignRight().Text("Đơn giá").FontColor(Colors.White).SemiBold();
                    header.Cell().Background("#0d6efd").Padding(6).AlignRight().Text("Thành tiền").FontColor(Colors.White).SemiBold();
                });

                // Dữ liệu dòng
                foreach (var detail in order.OrderDetails)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Text(detail.Tour?.Name ?? "Tour");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).AlignCenter().Text(detail.Quantity.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).AlignRight().Text($"{detail.UnitPrice:N0} đ");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8).AlignRight().Text($"{detail.SubTotal:N0} đ");
                }
            });
        }
    }
}
