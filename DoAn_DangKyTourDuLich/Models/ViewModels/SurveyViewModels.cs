using DoAn_DangKyTourDuLich.Models;
using System.ComponentModel.DataAnnotations;

namespace DoAn_DangKyTourDuLich.Models.ViewModels
{
    // Cấu trúc mô phỏng bộ câu hỏi
    public class SurveyQuestion
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }

    // Câu trả lời cho từng câu hỏi
    public class SurveyAnswer
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        
        // --- CÁC TRỌNG SỐ (WEIGHTS) ĐƯỢC CHẤM KHI CHỌN ĐÁP ÁN NÀY ---
        
        // Nếu chọn đáp án này, Tour có CategoryId tương ứng được cộng bao nhiêu điểm
        public Dictionary<int, int> CategoryWeights { get; set; } = new Dictionary<int, int>();
        
        // Khoảng giá (Ví dụ: Thích đi rẻ -> MaxBudget = 3000000. Nếu Tour < 3000000 -> cộng điểm)
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int BudgetWeight { get; set; } = 0; // Số điểm cộng nếu khớp ngân sách

        // Phương tiện (Nếu tour có phương tiện chứa từ khóa này thì được cộng điểm)
        public string? PreferredTransport { get; set; }
        public int TransportWeight { get; set; } = 0;

        // Thời gian (Duration)
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public int DurationWeight { get; set; } = 0;

        // Điểm đến (Nếu tour có điểm đến chứa từ khóa này thì cộng điểm)
        public List<string> PreferredDestinations { get; set; } = new List<string>();
        public int DestinationWeight { get; set; } = 0;
    }

    // Model dùng để Submit từ UI lên Controller
    public class SurveySubmissionViewModel
    {
        // Dictionary chứa <Id câu hỏi, Id câu trả lời>
        public Dictionary<int, int> Answers { get; set; } = new Dictionary<int, int>();
    }

    // Model kết quả trả về cho View
    public class SurveyResultViewModel
    {
        public List<RecommendedTour> RecommendedTours { get; set; } = new List<RecommendedTour>();
    }

    // Quản lý từng Tour và số % phù hợp
    public class RecommendedTour
    {
        public Tour Tour { get; set; } = null!;
        public int TotalScore { get; set; }
        public double MatchPercentage { get; set; }
    }
}
