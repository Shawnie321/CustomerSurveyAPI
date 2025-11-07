using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId}/questions")]
    public class SurveyQuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveyQuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/surveys/{surveyId}/questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions(int surveyId)
        {
            var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == surveyId);
            if (!surveyExists) return NotFound("Survey not found.");

            var questions = await _context.SurveyQuestions
                .Where(q => q.SurveyId == surveyId)
                .Include(q => q.Answers)
                .OrderBy(q => q.Id)
                .ToListAsync();

            var result = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType,
                Options = q.Options,
                AnswerCount = q.Answers?.Count ?? 0
            });

            return Ok(result);
        }

        // POST /api/surveys/{surveyId}/questions  (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddQuestion(int surveyId, [FromBody] CreateQuestionRequest request)
        {
            if (!await _context.Surveys.AnyAsync(s => s.Id == surveyId))
                return NotFound("Survey not found.");

            if (string.IsNullOrWhiteSpace(request.QuestionText))
                return BadRequest("QuestionText is required.");

            var question = new SurveyQuestion
            {
                SurveyId = surveyId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType ?? "Text",
                Options = request.Options
            };

            _context.SurveyQuestions.Add(question);
            await _context.SaveChangesAsync();

            var dto = new QuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                Options = question.Options,
                AnswerCount = 0
            };

            return CreatedAtAction(nameof(GetQuestions), new { surveyId }, dto);
        }

        // DELETE /api/surveys/{surveyId}/questions/{id}  (Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int surveyId, int id)
        {
            var question = await _context.SurveyQuestions
                .FirstOrDefaultAsync(q => q.SurveyId == surveyId && q.Id == id);

            if (question == null) return NotFound();

            // Remove answers first
            var answers = await _context.SurveyAnswers
                .Where(a => a.QuestionId == question.Id)
                .ToListAsync();
            if (answers.Any())
                _context.SurveyAnswers.RemoveRange(answers);

            _context.SurveyQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DTOs
        public class QuestionDto
        {
            public int Id { get; set; }
            public string QuestionText { get; set; } = "";
            public string QuestionType { get; set; } = "Text";
            public string? Options { get; set; }
            public int AnswerCount { get; set; }
        }

        public class CreateQuestionRequest
        {
            public string QuestionText { get; set; } = "";
            public string? QuestionType { get; set; } = "Text";
            public string? Options { get; set; }
        }
    }
}
