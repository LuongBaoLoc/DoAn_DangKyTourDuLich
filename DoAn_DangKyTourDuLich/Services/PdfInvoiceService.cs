using DoAn_DangKyTourDuLich.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DoAn_DangKyTourDuLich.Services
{
    public class PdfInvoiceService
    {
        public PdfInvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateInvoice(Order order)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(compose => ComposeHeader(compose, order));
                    page.Content().Element(compose => ComposeContent(compose, order));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Order order)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("HÓA ĐƠN ĐẶT TOUR").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text($"Mã Đơn: {order.OrderCode}").FontSize(14).SemiBold();
                    column.Item().Text($"Ngày tạo: {order.OrderDate:dd/MM/yyyy HH:mm}");
                    column.Item().PaddingTop(5).Text("Công ty Du Lịch XYZ").SemiBold();
                    column.Item().Text("123 Đường Cờ Cờ, TP.HCM");
                });
                // row.ConstantItem(100).Height(50).Placeholder(); // Placeholder for logo
            });
        }

        private void ComposeContent(IContainer container, Order order)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new AddressComponent("Thông tin khách hàng", order.CustomerName, order.CustomerEmail, order.CustomerPhone, order.CustomerAddress));
                });

                column.Item().Element(c => ComposeTable(c, order));

                column.Item().AlignRight().Text($"Tổng tiền: {order.TotalAmount:N0} VNĐ").FontSize(16).SemiBold();
                
                column.Item().PaddingTop(10).Text(t =>
                {
                    t.Span("Trạng thái: ").SemiBold();
                    string statusTxt = order.Status switch
                    {
                        OrderStatus.Pending => "Chờ xác nhận",
                        OrderStatus.Confirmed => "Đã xác nhận",
                        OrderStatus.Completed => "Hoàn thành",
                        OrderStatus.Cancelled => "Đã hủy",
                        _ => order.Status.ToString()
                    };
                    t.Span(statusTxt).FontColor(order.Status == OrderStatus.Completed ? Colors.Green.Medium : Colors.Orange.Medium);
                });
            });
        }

        private void ComposeTable(IContainer container, Order order)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Tên Tour");
                    header.Cell().Element(CellStyle).AlignRight().Text("Đơn giá");
                    header.Cell().Element(CellStyle).AlignRight().Text("Số lượng");
                    header.Cell().Element(CellStyle).AlignRight().Text("Thành tiền");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                int i = 1;
                foreach (var detail in order.OrderDetails)
                {
                    table.Cell().Element(CellStyle).Text(i++.ToString());
                    table.Cell().Element(CellStyle).Text(detail.Tour?.Name ?? "Tour");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{detail.UnitPrice:N0} VNĐ");
                    table.Cell().Element(CellStyle).AlignRight().Text(detail.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"{detail.SubTotal:N0} VNĐ");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }
    }

    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private string Name { get; }
        private string Email { get; }
        private string Phone { get; }
        private string? Address { get; }

        public AddressComponent(string title, string name, string email, string phone, string? address)
        {
            Title = title;
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item().BorderBottom(1).PaddingBottom(5).Text(Title).SemiBold();
                column.Item().Text(Name);
                column.Item().Text(Email);
                column.Item().Text(Phone);
                if (!string.IsNullOrEmpty(Address))
                    column.Item().Text(Address);
            });
        }
    }
}
