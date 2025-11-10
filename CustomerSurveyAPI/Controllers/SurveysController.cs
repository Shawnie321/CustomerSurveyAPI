using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Models;
using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET /api/surveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyReadDto>>> GetSurveys()
        {
            var surveys = (await _surveyService.GetAllAsync())
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SurveyReadDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    CreatedBy = s.CreatedBy,
                    CreatedAt = s.CreatedAt,
                    Questions = s.Questions.Select(q => new SurveyQuestionReadDto
                    {
                        Id = q.Id,
                        SurveyId = q.SurveyId,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Options = q.Options,
                        IsRequired = q.IsRequired
                    }).ToList()
                })
                .ToList();

            return Ok(surveys);
        }

        // GET /api/surveys/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var s = await _surveyService.GetByIdAsync(id);
            if (s == null) return NotFound();

            var survey = new SurveyReadDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                CreatedBy = s.CreatedBy,
                CreatedAt = s.CreatedAt,
                Questions = s.Questions.OrderBy(q => q.Id).Select(q => new SurveyQuestionReadDto
                {
                    Id = q.Id,
                    SurveyId = q.SurveyId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = q.Options,
                    IsRequired = q.IsRequired
                }).ToList()
            };

            return Ok(survey);
        }

        // POST /api/surveys  (Accepts SurveyCreateDto with Questions array)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSurvey(SurveyCreateDto input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var survey = new Survey
            {
                Title = input.Title,
                Description = input.Description,
                CreatedBy = input.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Always add the privacy agreement question
            survey.Questions.Add(new SurveyQuestion
            {
                QuestionText = "Do you agree to the privacy terms and conditions?",
                QuestionType = "MultipleChoice",
                Options = "Yes,No",
                IsRequired = true
            });

            // Add other questions if provided
            if (input.Questions != null && input.Questions.Any())
            {
                foreach (var q in input.Questions)
                {
                    survey.Questions.Add(new SurveyQuestion
                    {
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Options = q.Options,
                        IsRequired = q.IsRequired
                    });
                }
            }

            var created = await _surveyService.CreateAsync(survey);

            // reload with includes to return DTO (service GetByIdAsync includes questions)
            var result = await _surveyService.GetByIdAsync(created.Id);

            var dto = new SurveyReadDto
            {
                Id = result!.Id,
                Title = result.Title,
                Description = result.Description,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                Questions = result.Questions.Select(q => new SurveyQuestionReadDto
                {
                    Id = q.Id,
                    SurveyId = q.SurveyId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Options = q.Options,
                    IsRequired = q.IsRequired
                }).ToList()
            };

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var deleted = await _surveyService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}