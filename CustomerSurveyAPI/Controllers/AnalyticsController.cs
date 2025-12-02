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

            // Only include responses with consent
            var consentedResponses = survey.Responses.Where(r => r.ConsentGiven).ToList();

            var allRatings = consentedResponses
                .SelectMany(r => r.Answers)
                .Where(a => a.RatingValue.HasValue)
                .Select(a => a.RatingValue.GetValueOrDefault())
                .ToList();

            var average = allRatings.Any() ? allRatings.Average() : 0;
            var highest = allRatings.Any() ? allRatings.Max() : 0;
            var lowest = allRatings.Any() ? allRatings.Min() : 0;

            var totalResponses = consentedResponses.Count;

            // Breakdown by question, only using consented answers
            var questionStats = survey.Questions.Select(q => new
            {
                QuestionId = q.Id,
                q.QuestionText,
                AverageRating = q.Answers
                    .Where(a => a.RatingValue.HasValue && a.SurveyResponse.ConsentGiven)
                    .Select(a => a.RatingValue.GetValueOrDefault())
                    .DefaultIfEmpty(0)
                    .Average(),
                ResponseCount = q.Answers.Count(a => a.SurveyResponse.ConsentGiven),
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