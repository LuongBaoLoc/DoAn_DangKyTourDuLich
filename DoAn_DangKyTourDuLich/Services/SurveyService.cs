using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Repositories.Interfaces;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Định nghĩa sẵn dữ liệu in-memory cho các câu hỏi.
        // Trong thực tế có thể đọc từ DB, nhưng để trình diễn thuật toán rõ nhất thì code in-memory là hợp lý.
        private readonly List<SurveyQuestion> _questions = new List<SurveyQuestion>
        {
            new SurveyQuestion
            {
                Id = 1,
                Text = "Ngân sách tối đa của bạn cho chuyến đi là bao nhiêu?",
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Id = 101, Text = "Dưới 3 triệu - Đi học/Sinh viên", MaxBudget = 3000000, BudgetWeight = 15 },
                    new SurveyAnswer { Id = 102, Text = "Từ 3 - 7 triệu - Bình dân", MinBudget = 3000000, MaxBudget = 7000000, BudgetWeight = 15 },
                    new SurveyAnswer { Id = 103, Text = "Trên 7 triệu - Nghỉ dưỡng thoải mái", MinBudget = 7000000, BudgetWeight = 15 },
                }
            },
            new SurveyQuestion
            {
                Id = 2,
                Text = "Bạn thích phong cách du lịch nào nhất?",
                Answers = new List<SurveyAnswer>
                {
                    // Giả định CategoryId: 1 = Biển đảo, 2 = Văn Trùng/Thiên nhiên (Khám phá), 3 = Nước Ngoài, 4 = Trọn gói, v.v.
                    // Cần map tương đối với Category của HT. Ở đây ta cộng điểm mạnh nếu đúng sở thích.
                    // Ta tạm dùng mô phỏng điểm số cho CategoryId 1, 2, 3...
                    new SurveyAnswer { Id = 201, Text = "Nghỉ dưỡng & Biển đảo thư giãn", CategoryWeights = new Dictionary<int, int>{ {1, 20}, {4, 10} } }, // Cộng 20đ nếu là category 1
                    new SurveyAnswer { Id = 202, Text = "Khám phá thám hiểm & Cắm trại", CategoryWeights = new Dictionary<int, int>{ {2, 20} } },
                    new SurveyAnswer { Id = 203, Text = "Du lịch kết hợp tâm linh / Văn hóa", CategoryWeights = new Dictionary<int, int>{ {3, 20} } },
                    new SurveyAnswer { Id = 204, Text = "Đi đâu cũng được, miễn đông vui!", CategoryWeights = new Dictionary<int, int>{ {1, 5}, {2, 5}, {3, 5}, {4, 5} } }
                }
            },
            new SurveyQuestion
            {
                Id = 3,
                Text = "Vùng miền bạn mong muốn đặt chân đến?",
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Id = 301, Text = "Phía Bắc (Sapa, Hạ Long, Hà Nội...)", 
                                       PreferredDestinations = new List<string> { "Sapa", "Hạ Long", "Hà Nội", "Ninh Bình", "Hà Giang" }, DestinationWeight = 15 },
                    new SurveyAnswer { Id = 302, Text = "Miền Trung (Đà Nẵng, Huế, Hội An...)", 
                                       PreferredDestinations = new List<string> { "Đà Nẵng", "Huế", "Hội An", "Nha Trang", "Phú Yên" }, DestinationWeight = 15 },
                    new SurveyAnswer { Id = 303, Text = "Phía Nam (Miền Tây, Vũng Tàu, Phú Quốc...)", 
                                       PreferredDestinations = new List<string> { "Phú Quốc", "Vũng Tàu", "Miền Tây", "Đà Lạt", "Côn Đảo" }, DestinationWeight = 15 },
                    new SurveyAnswer { Id = 304, Text = "Ra nước ngoài chơi!", 
                                       PreferredDestinations = new List<string> { "Thái Lan", "Hàn Quốc", "Nhật Bản", "Châu Âu", "Mỹ", "Singapore" }, DestinationWeight = 15 }
                }
            },
             new SurveyQuestion
            {
                Id = 4,
                Text = "Phương tiện di chuyển mà bạn chọn?",
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Id = 401, Text = "Máy bay (Nhanh chóng, tiện lợi)", PreferredTransport = "Máy bay", TransportWeight = 10 },
                    new SurveyAnswer { Id = 402, Text = "Ô tô / Xe khách (Thoải mái ngắm cảnh)", PreferredTransport = "Xe", TransportWeight = 10 },
                    new SurveyAnswer { Id = 403, Text = "Tàu hỏa (Trải nghiệm thú vị)", PreferredTransport = "Tàu", TransportWeight = 10 }
                }
            },
            new SurveyQuestion
            {
                Id = 5,
                Text = "Bạn có bao nhiêu ngày cho chuyến đi này?",
                Answers = new List<SurveyAnswer>
                {
                    new SurveyAnswer { Id = 501, Text = "1 - 2 ngày (Cuối tuần)", MinDuration = 1, MaxDuration = 2, DurationWeight = 15 },
                    new SurveyAnswer { Id = 502, Text = "3 - 5 ngày (Kỳ nghỉ chuẩn)", MinDuration = 3, MaxDuration = 5, DurationWeight = 15 },
                    new SurveyAnswer { Id = 503, Text = "Hơn 5 ngày (Phượt dài ngày / Xuyên Việt)", MinDuration = 6, MaxDuration = 365, DurationWeight = 15 }
                }
            }
        };

        public SurveyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<SurveyQuestion> GetQuestions()
        {
            return _questions;
        }

        public async Task<SurveyResultViewModel> ProcessSurveyAsync(SurveySubmissionViewModel submission)
        {
            // Bước 1: Lấy tất cả Tour đang Active
            var activeTours = await _unitOfWork.Tours.GetActiveToursAsync();

            var startScores = new Dictionary<int, int>();
            foreach (var t in activeTours) startScores[t.Id] = 0;

            int maxPossibleScore = 0;

            // Bước 2: Duyệt qua từng câu trả lời gửi lên để tính điểm
            foreach (var qa in submission.Answers)
            {
                int qId = qa.Key;
                int aId = qa.Value;

                var question = _questions.FirstOrDefault(q => q.Id == qId);
                var answer = question?.Answers.FirstOrDefault(a => a.Id == aId);

                if (answer == null) continue;

                // Tính Maximum point có thể đạt được từ câu hỏi này
                int localMax = Math.Max(answer.BudgetWeight, 
                               Math.Max(answer.CategoryWeights.Values.Any() ? answer.CategoryWeights.Values.Max() : 0, 
                               Math.Max(answer.DestinationWeight, 
                               Math.Max(answer.TransportWeight, answer.DurationWeight))));
                maxPossibleScore += localMax;

                // Cập nhật điểm cho từng Tour
                foreach (var tour in activeTours)
                {
                    // 1. Chấm điểm Ngân sách
                    if (answer.BudgetWeight > 0)
                    {
                        bool matchBudget = true;
                        decimal price = tour.DisplayPrice;
                        if (answer.MinBudget.HasValue && price < answer.MinBudget.Value) matchBudget = false;
                        if (answer.MaxBudget.HasValue && price > answer.MaxBudget.Value) matchBudget = false;

                        if (matchBudget) startScores[tour.Id] += answer.BudgetWeight;
                    }

                    // 2. Chấm điểm Category (Sở thích)
                    if (answer.CategoryWeights.ContainsKey(tour.CategoryId))
                    {
                        startScores[tour.Id] += answer.CategoryWeights[tour.CategoryId];
                    }

                    // 3. Chấm điểm Vùng miền / Destination
                    if (answer.DestinationWeight > 0 && answer.PreferredDestinations.Any() && !string.IsNullOrEmpty(tour.Destination))
                    {
                        bool matchDest = answer.PreferredDestinations.Any(d => 
                            tour.Destination.Contains(d, StringComparison.OrdinalIgnoreCase) || 
                            tour.Name.Contains(d, StringComparison.OrdinalIgnoreCase));
                            
                        if (matchDest) startScores[tour.Id] += answer.DestinationWeight;
                    }

                    // 4. Chấm điểm Phương tiện
                    if (answer.TransportWeight > 0 && !string.IsNullOrEmpty(answer.PreferredTransport) && !string.IsNullOrEmpty(tour.Transportation))
                    {
                        if (tour.Transportation.Contains(answer.PreferredTransport, StringComparison.OrdinalIgnoreCase))
                        {
                            startScores[tour.Id] += answer.TransportWeight;
                        }
                    }

                    // 5. Chấm điểm Thời gian (Duration)
                    if (answer.DurationWeight > 0)
                    {
                        bool matchDuration = true;
                        if (answer.MinDuration.HasValue && tour.Duration < answer.MinDuration.Value) matchDuration = false;
                        if (answer.MaxDuration.HasValue && tour.Duration > answer.MaxDuration.Value) matchDuration = false;

                        if (matchDuration) startScores[tour.Id] += answer.DurationWeight;
                    }
                }
            }

            if (maxPossibleScore == 0) maxPossibleScore = 1; // Prevent div by 0;

            // Bước 3: Sắp xếp lấy Top 6 tour phù hợp nhất (Chỉ lấy tour có ít nhất 1 điểm)
            var recommendations = activeTours
                .Where(t => startScores[t.Id] > 0)
                .Select(t => new RecommendedTour
                {
                    Tour = t,
                    TotalScore = startScores[t.Id],
                    // Ép tỷ lệ Match %, tối đa 98% (để trông tự nhiên, ko phải máy móc tuyệt đối)
                    MatchPercentage = Math.Min(98, Math.Round(((double)startScores[t.Id] / maxPossibleScore) * 100))
                })
                .OrderByDescending(x => x.TotalScore)
                .ThenByDescending(x => x.Tour.IsFeatured)
                .Take(6)
                .ToList();

            // Nếu Match == 0, ta có thể lấy đại tour HOT để không bị trang trắng
            if (!recommendations.Any())
            {
                recommendations = activeTours
                    .Where(t => t.IsFeatured)
                    .Take(3)
                    .Select(t => new RecommendedTour
                    {
                        Tour = t,
                        TotalScore = 0,
                        MatchPercentage = 15 // Gợi ý mặc định
                    }).ToList();
            }

            return new SurveyResultViewModel
            {
                RecommendedTours = recommendations
            };
        }
    }
}
