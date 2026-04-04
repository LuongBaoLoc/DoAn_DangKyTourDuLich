using DoAn_DangKyTourDuLich.Models.ViewModels;
using DoAn_DangKyTourDuLich.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_DangKyTourDuLich.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: /Survey
        public IActionResult Index()
        {
            var questions = _surveyService.GetQuestions();
            return View(questions);
        }

        // POST: /Survey/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SurveySubmissionViewModel submission)
        {
            if (submission == null || submission.Answers == null || !submission.Answers.Any())
            {
                TempData["Error"] = "Bạn chưa hoàn thành bộ câu hỏi khảo sát.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _surveyService.ProcessSurveyAsync(submission);
            
            // Thay vì Redirect, ta trả luôn View Results kèm Model cho đỡ phức tạp việc Serialize vào TempData
            return View("Results", result);
        }
    }
}
