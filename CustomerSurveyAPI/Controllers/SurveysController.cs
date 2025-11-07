using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveysController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/surveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSurveys()
        {
            var surveys = await _context.Surveys
                .Include(s => s.Questions)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Description,
                    s.CreatedBy,
                    s.CreatedAt,
                    Questions = s.Questions.Select(q => new {
                        q.Id,
                        q.QuestionText,
                        q.QuestionType,
                        q.Options
                    })
                })
                .ToListAsync();

            return Ok(surveys);
        }

        // GET /api/surveys/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .Where(s => s.Id == id)
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Description,
                    s.CreatedBy,
                    s.CreatedAt,
                    Questions = s.Questions.OrderBy(q => q.Id).Select(q => new {
                        q.Id,
                        q.QuestionText,
                        q.QuestionType,
                        q.Options
                    })
                })
                .FirstOrDefaultAsync();

            if (survey == null) return NotFound();
            return Ok(survey);
        }

        // POST /api/surveys  (Accepts Survey with Questions array)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSurvey([FromBody] Survey input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var survey = new Survey
            {
                Title = input.Title,
                Description = input.Description,
                CreatedBy = input.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Add questions if provided
            if (input.Questions != null && input.Questions.Any())
            {
                foreach (var q in input.Questions)
                {
                    survey.Questions.Add(new SurveyQuestion
                    {
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Options = q.Options
                    });
                }
            }

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            // return the saved survey including generated IDs for questions
            var result = await _context.Surveys
                .Include(s => s.Questions)
                .Where(s => s.Id == survey.Id)
                .Select(s => new {
                    s.Id,
                    s.Title,
                    s.Description,
                    s.CreatedBy,
                    s.CreatedAt,
                    Questions = s.Questions.Select(q => new {
                        q.Id,
                        q.QuestionText,
                        q.QuestionType,
                        q.Options
                    })
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null) return NotFound();

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
} 