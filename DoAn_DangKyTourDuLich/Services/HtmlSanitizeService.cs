using Ganss.Xss;

namespace DoAn_DangKyTourDuLich.Services
{
    public class HtmlSanitizeService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizeService()
        {
            _sanitizer = new HtmlSanitizer();
            // Cấu hình thêm thuộc tính hoặc tag nếu cần
            _sanitizer.AllowedAttributes.Add("class");
            _sanitizer.AllowedAttributes.Add("style");
            _sanitizer.AllowedCssProperties.Add("color");
            _sanitizer.AllowedCssProperties.Add("background-color");
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;
            return _sanitizer.Sanitize(html);
        }
    }
}
