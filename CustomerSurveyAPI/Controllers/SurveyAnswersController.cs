using AutoMapper;
using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class SurveyAnswersController : ControllerBase
    {
        private readonly ISurveyAnswerService _answerService;
        private readonly ISurveyResponseService _responseService;
        private readonly ISurveyService _surveyService;
        private readonly IMapper _mapper;

        public SurveyAnswersController(
            ISurveyAnswerService answerService,
            ISurveyResponseService responseService,
            ISurveyService surveyService,
            IMapper mapper)
        {
            _answerService = answerService;
            _responseService = responseService;
            _surveyService = surveyService;
            _mapper = mapper;
        }

        // GET /api/answers/{id}
        // Admins can fetch any answer. Non-admins can fetch only answers belonging to their own response.
        [Authorize]
        [HttpGet("answers/{id}")]
        public async Task<IActionResult> GetAnswerById(int id)
        {
            var answer = await _answerService.GetByIdAsync(id);
            if (answer == null) return NotFound();

            var response = await _responseService.GetByIdAsync(answer.SurveyResponseId);
            if (response == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var username = User?.Identity?.Name;
                if (string.IsNullOrWhiteSpace(username) || !string.Equals(username, response.Username, StringComparison.Ordinal))
                    return Forbid();
            }

            var dto = _mapper.Map<SurveyAnswerReadDto>(answer);
            return Ok(dto);
        }

        // GET /api/surveys/{surveyId}/answers
        // Admin only: returns all answers for a survey (across all responses)
        [Authorize(Roles = "Admin")]
        [HttpGet("surveys/{surveyId}/answers")]
        public async Task<IActionResult> GetAnswersForSurvey(int surveyId)
        {
            var survey = await _surveyService.GetByIdAsync(surveyId);
            if (survey == null) return NotFound("Survey not found.");

            var responses = await _responseService.GetBySurveyIdAsync(surveyId);

            var answers = responses
                .Where(r => r.Answers != null)
                .SelectMany(r => r.Answers!)
                .Select(a => _mapper.Map<SurveyAnswerReadDto>(a))
                .ToList();

            return Ok(answers);
        }

        // GET /api/responses/{responseId}/answers
        // Authenticated: admins can fetch any, non-admins only their own response answers
        [Authorize]
        [HttpGet("responses/{responseId}/answers")]
        public async Task<IActionResult> GetAnswersByResponseId(int responseId)
        {
            var response = await _responseService.GetByIdAsync(responseId); // keep consistent casing in code below
            if (response == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var username = User?.Identity?.Name;
                if (string.IsNullOrWhiteSpace(username) || !string.Equals(username, response.Username, StringComparison.Ordinal))
                    return Forbid();
            }

            var answers = response.Answers?
                .Select(a => _mapper.Map<SurveyAnswerReadDto>(a))
                .ToList() ?? new List<SurveyAnswerReadDto>();

            return Ok(answers);
        }
    }
}