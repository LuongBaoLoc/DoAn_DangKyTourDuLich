namespace DoAn_DangKyTourDuLich.Services
{
    /// <summary>
    /// Dịch vụ lọc từ cấm trong nội dung (tiếng Việt)
    /// Quét comment để phát hiện ngôn từ độc hại, spam, hoặc vi phạm tiêu chuẩn cộng đồng
    /// </summary>
    public class ProfanityFilterService
    {
        private readonly HashSet<string> _bannedWords;
        private readonly HashSet<string> _spamPatterns;

        public ProfanityFilterService()
        {
            // Danh sách từ cấm (tiếng Việt)
            // Thêm các từ cấm tùy theo tiêu chuẩn của công ty
            _bannedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Từ chửi rủa
                "đụ", "đéo", "chết", "khốn", "lao", "điên", "ngu", "nứt", "chó",
                "mẹ kiếp", "thằng", "con", "nít", "cục", "hang", "dóc", "lược",
                
                // Spam, lừa đảo
                "tìm việc dễ", "kiếm tiền nhanh", "lên đơn", "rút tiền", "nạp tiền",
                "liên hệ zalo", "liên hệ facebook", "call me", "contact me",
                "click link", "truy cập", "vào trang", "link:", "http", "www",
                
                // Quảng cáo
                "mua ngay", "liên hệ shop", "order now", "gọi ngay",
                
                // Ngoài lề
                "bán hàng", "bán tour", "tour khác", "khuyến mãi",
            };

            // Các mẫu regex cho phát hiện spam
            _spamPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                @"https?://", // URL
                @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)", // IP
                @"\+\d{1,3}\s?\d{1,14}", // Số điện thoại quốc tế
                @"(?<![\w\.])[0-9]{10,}", // Số điện thoại dài
                @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}", // Email
                @"[❤❤❤❤❤|★★★★★|😍😍😍]", // Spam emoji
            };
        }

        /// <summary>
        /// Kiểm tra xem comment có chứa từ cấm hay không
        /// </summary>
        public FilterResult FilterContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new FilterResult { IsClean = true };

            var lowerContent = content.ToLower();
            var foundBannedWords = new List<string>();
            var detectedSpam = false;

            // Kiểm tra từ cấm
            foreach (var word in _bannedWords)
            {
                // Tìm từ với biên giới từ để tránh false positive
                if (System.Text.RegularExpressions.Regex.IsMatch(lowerContent, $@"\b{System.Text.RegularExpressions.Regex.Escape(word)}\b"))
                {
                    foundBannedWords.Add(word);
                }

                // Hoặc tìm đơn giản nếu là cụm từ
                if (word.Contains(" ") && lowerContent.Contains(word))
                {
                    foundBannedWords.Add(word);
                }
            }

            // Kiểm tra mẫu spam
            foreach (var pattern in _spamPatterns)
            {
                try
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern))
                    {
                        detectedSpam = true;
                        break;
                    }
                }
                catch
                {
                    // Bỏ qua pattern không hợp lệ
                }
            }

            // Kiểm tra ký tự lặp quá nhiều (spam emoji/ký tự)
            if (HasExcessiveRepeatedCharacters(content))
                detectedSpam = true;

            // Kiểm tra độ dài quá ngắn hoặc quá dài
            if (content.Length < 10)
                return new FilterResult 
                { 
                    IsClean = false, 
                    Reason = "Nhận xét quá ngắn (tối thiểu 10 ký tự)" 
                };

            if (content.Length > 1000)
                return new FilterResult 
                { 
                    IsClean = false, 
                    Reason = "Nhận xét quá dài (tối đa 1000 ký tự)" 
                };

            // Kết quả
            if (foundBannedWords.Count > 0)
            {
                return new FilterResult
                {
                    IsClean = false,
                    Reason = $"Nhận xét chứa từngữ cấm: {string.Join(", ", foundBannedWords)}",
                    BannedWordsFound = foundBannedWords
                };
            }

            if (detectedSpam)
            {
                return new FilterResult
                {
                    IsClean = false,
                    Reason = "Nhận xét chứa nội dung spam hoặc liên kết"
                };
            }

            return new FilterResult { IsClean = true };
        }

        /// <summary>
        /// Kiểm tra ký tự lặp quá nhiều (thường là spam)
        /// Ví dụ: "😍😍😍😍😍" hoặc "oooooooooooo"
        /// </summary>
        private bool HasExcessiveRepeatedCharacters(string content)
        {
            var groups = System.Text.RegularExpressions.Regex.Matches(content, @"(.)\1{4,}");
            return groups.Count > 2; // Nếu có nhiều hơn 2 nhóm ký tự lặp liên tiếp 5+ lần
        }

        /// <summary>
        /// Làm sạch comment bằng cách thay thế/ẩn từ cấm
        /// </summary>
        public string CleanContent(string content)
        {
            var cleaned = content;

            foreach (var word in _bannedWords)
            {
                cleaned = System.Text.RegularExpressions.Regex.Replace(
                    cleaned,
                    System.Text.RegularExpressions.Regex.Escape(word),
                    new string('*', Math.Min(word.Length, 5)),
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return cleaned;
        }
    }

    public class FilterResult
    {
        public bool IsClean { get; set; }
        public string? Reason { get; set; }
        public List<string>? BannedWordsFound { get; set; }
    }
}
