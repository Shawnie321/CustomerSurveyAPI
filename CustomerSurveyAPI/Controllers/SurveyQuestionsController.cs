using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerSurveyAPI.Models;
using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Services;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId}/questions")]
    public class SurveyQuestionsController : ControllerBase
    {
        private readonly ISurveyQuestionService _questionService;
        private readonly ISurveyAnswerService _answerService;
        private readonly ISurveyService _surveyService;

        public SurveyQuestionsController(ISurveyQuestionService questionService, ISurveyAnswerService answerService, ISurveyService surveyService)
        {
            _questionService = questionService;
            _answerService = answerService;
            _surveyService = surveyService;
        }

        // GET /api/surveys/{surveyId}/questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyQuestionReadDto>>> GetQuestions(int surveyId)
        {
            var survey = await _surveyService.GetByIdAsync(surveyId);
            if (survey == null) return NotFound("Survey not found.");

            var questions = await _questionService.GetBySurveyIdAsync(surveyId);

            var result = questions.Select(q => new SurveyQuestionReadDto
            {
                Id = q.Id,
                SurveyId = q.SurveyId,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType,
                Options = q.Options,
                IsRequired = q.IsRequired,
                AnswerCount = q.Answers?.Count ?? 0
            });

            return Ok(result);
        }

        // POST /api/surveys/{surveyId}/questions  (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddQuestion(int surveyId, SurveyQuestionCreateDto request)
        {
            if (await _surveyService.GetByIdAsync(surveyId) == null)
                return NotFound("Survey not found.");

            if (string.IsNullOrWhiteSpace(request.QuestionText))
                return BadRequest("QuestionText is required.");

            var question = new SurveyQuestion
            {
                SurveyId = surveyId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType ?? "Text",
                Options = request.Options,
                IsRequired = request.IsRequired
            };

            var created = await _questionService.CreateAsync(question);

            var dto = new SurveyQuestionReadDto
            {
                Id = created.Id,
                SurveyId = created.SurveyId,
                QuestionText = created.QuestionText,
                QuestionType = created.QuestionType,
                Options = created.Options,
                IsRequired = created.IsRequired,
                AnswerCount = created.Answers?.Count ?? 0
            };

            return CreatedAtAction(nameof(GetQuestions), new { surveyId }, dto);
        }

        // DELETE /api/surveys/{surveyId}/questions/{id}  (Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int surveyId, int id)
        {
            var question = await _questionService.GetByIdAsync(id);

            if (question == null || question.SurveyId != surveyId) return NotFound();

            // Remove answers first
            await _answerService.DeleteByQuestionIdAsync(question.Id);

            await _questionService.DeleteAsync(question.Id);

            return NoContent();
        }
    }
}
