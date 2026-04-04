using DoAn_DangKyTourDuLich.Models.ViewModels;

namespace DoAn_DangKyTourDuLich.Services.Interfaces
{
    public interface ISurveyService
    {
        List<SurveyQuestion> GetQuestions();
        Task<SurveyResultViewModel> ProcessSurveyAsync(SurveySubmissionViewModel submission);
    }
}
