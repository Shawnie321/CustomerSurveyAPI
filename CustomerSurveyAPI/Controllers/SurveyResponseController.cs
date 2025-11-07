using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId}/responses")]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveyResponsesController(AppDbContext context)
        {
            _context = context;
        }

        // ---------- GET RESPONSES (Admin only) ----------
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetResponses(int surveyId)
        {
            var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == surveyId);
            if (!surveyExists) return NotFound("Survey not found.");

            var responses = await _context.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .Include(r => r.Answers)
                    .ThenInclude(a => a.SurveyQuestion)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();

            var result = responses.Select(r => new SurveyResponseDto
            {
                Id = r.Id,
                SurveyId = r.SurveyId,
                Username = r.Username,
                SubmittedAt = r.SubmittedAt,
                Answers = r.Answers.Select(a => new SurveyAnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    QuestionText = a.SurveyQuestion?.QuestionText,
                    AnswerText = a.AnswerText,
                    RatingValue = a.RatingValue
                }).ToList()
            });

            return Ok(result);
        }

        // ---------- SUBMIT A RESPONSE ----------
        // Accept a compact payload to avoid requiring navigation properties
        public class SubmitAnswerRequest
        {
            public int QuestionId { get; set; }
            public string? AnswerText { get; set; }
            public int? RatingValue { get; set; }
        }

        public class SubmitResponseRequest
        {
            public string Username { get; set; } = "Anonymous";
            public List<SubmitAnswerRequest>? Answers { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostResponse(int surveyId, [FromBody] SubmitResponseRequest input)
        {
            if (input == null) return BadRequest("Invalid payload.");

            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey == null) return NotFound("Survey not found.");

            // Optional: validate question IDs exist and belong to this survey
            var questionIds = (input.Answers ?? Enumerable.Empty<SubmitAnswerRequest>())
                                .Select(a => a.QuestionId)
                                .Distinct()
                                .ToList();

            if (questionIds.Any())
            {
                var validCount = await _context.SurveyQuestions
                    .Where(q => questionIds.Contains(q.Id) && q.SurveyId == surveyId)
                    .CountAsync();

                if (validCount != questionIds.Count)
                    return BadRequest("One or more question IDs are invalid for this survey.");
            }

            var response = new SurveyResponse
            {
                SurveyId = surveyId,
                Username = input.Username,
                SubmittedAt = DateTime.UtcNow,
                Answers = new List<SurveyAnswer>()
            };

            if (input.Answers != null)
            {
                foreach (var a in input.Answers)
                {
                    response.Answers.Add(new SurveyAnswer
                    {
                        QuestionId = a.QuestionId,
                        AnswerText = string.IsNullOrWhiteSpace(a.AnswerText) ? null : a.AnswerText,
                        RatingValue = a.RatingValue
                    });
                }
            }

            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();

            var createdDto = new SurveyResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                Username = response.Username,
                SubmittedAt = response.SubmittedAt,
                Answers = response.Answers.Select(a => new SurveyAnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    QuestionText = null, // Question text not loaded here
                    AnswerText = a.AnswerText,
                    RatingValue = a.RatingValue
                }).ToList()
            };

            return CreatedAtAction(nameof(GetResponses), new { surveyId = surveyId }, createdDto);
        }

        // ---------- DELETE RESPONSE (Admin) ----------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResponse(int surveyId, int id)
        {
            var response = await _context.SurveyResponses
                .FirstOrDefaultAsync(r => r.SurveyId == surveyId && r.Id == id);

            if (response == null) return NotFound();

            // Remove answers first (cascade should do it but be explicit)
            var answers = await _context.SurveyAnswers
                .Where(a => a.SurveyResponseId == response.Id)
                .ToListAsync();

            if (answers.Any())
                _context.SurveyAnswers.RemoveRange(answers);

            _context.SurveyResponses.Remove(response);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DTOs
        public class SurveyResponseDto
        {
            public int Id { get; set; }
            public int SurveyId { get; set; }
            public string Username { get; set; } = "";
            public DateTime SubmittedAt { get; set; }
            public List<SurveyAnswerDto> Answers { get; set; } = new();
        }

        public class SurveyAnswerDto
        {
            public int Id { get; set; }
            public int QuestionId { get; set; }
            public string? QuestionText { get; set; }
            public string? AnswerText { get; set; }
            public int? RatingValue { get; set; }
        }
    }
}
