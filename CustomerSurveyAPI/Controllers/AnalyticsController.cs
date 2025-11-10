using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public AnalyticsController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // ---------- GET ANALYTICS FOR A SURVEY ----------
        [Authorize(Roles = "Admin")]
        [HttpGet("{surveyId}")]
        public async Task<IActionResult> GetSurveyAnalytics(int surveyId)
        {
            var survey = await _surveyService.GetByIdAsync(surveyId);

            if (survey == null)
                return NotFound("Survey not found.");

            var allRatings = survey.Responses
                .SelectMany(r => r.Answers)
                .Where(a => a.RatingValue.HasValue)
                .Select(a => a.RatingValue.GetValueOrDefault())
                .ToList();

            var average = allRatings.Any() ? allRatings.Average() : 0;
            var highest = allRatings.Any() ? allRatings.Max() : 0;
            var lowest = allRatings.Any() ? allRatings.Min() : 0;

            var totalResponses = survey.Responses.Count;

            // Breakdown by question
            var questionStats = survey.Questions.Select(q => new
            {
                QuestionId = q.Id,
                q.QuestionText,
                AverageRating = q.Answers.Where(a => a.RatingValue.HasValue).Select(a => a.RatingValue.GetValueOrDefault()).DefaultIfEmpty(0).Average(),
                ResponseCount = q.Answers.Count,
                Type = q.QuestionType
            });

            return Ok(new
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                TotalResponses = totalResponses,
                AverageRating = Math.Round(average, 2),
                HighestRating = highest,
                LowestRating = lowest,
                Questions = questionStats
            });
        }
    }
}