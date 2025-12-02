using AutoMapper;
using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Models;
using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/surveys/{surveyId}/responses")]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly ISurveyResponseService _responseService;
        private readonly ISurveyService _surveyService;
        private readonly ISurveyQuestionService _questionService;
        private readonly IMapper _mapper;
        private const string PrivacyQuestionText = "Do you agree to the privacy terms and conditions?";

        public SurveyResponsesController(ISurveyResponseService responseService, ISurveyService surveyService, ISurveyQuestionService questionService, IMapper mapper)
        {
            _responseService = responseService;
            _surveyService = surveyService;
            _questionService = questionService;
            _mapper = mapper;
        }

        // ---------- GET RESPONSES (Admin only) ----------
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyResponseReadDto>>> GetResponses(int surveyId)
        {
            var survey = await _surveyService.GetByIdAsync(surveyId);
            if (survey == null) return NotFound("Survey not found.");

            var responses = await _responseService.GetBySurveyIdAsync(surveyId);

            var result = responses.Select(r => _mapper.Map<SurveyResponseReadDto>(r));

            return Ok(result);
        }

        // ---------- SUBMIT A RESPONSE ----------
        [HttpPost]
        public async Task<IActionResult> SubmitResponse(int surveyId, [FromBody] SubmitResponseDto input)
        {
            if (input == null) return BadRequest("Invalid payload.");

            var survey = await _surveyService.GetByIdAsync(surveyId);
            if (survey == null) return NotFound("Survey not found.");

            if (!input.ConsentGiven)
            {
                return BadRequest("Consent is required to submit the survey.");
            }

            // Optional: validate question IDs exist and belong to this survey
            var questionIds = (input.Answers ?? Enumerable.Empty<SubmitAnswerDto>())
                                .Select(a => a.QuestionId)
                                .Distinct()
                                .ToList();

            var surveyQuestionIds = survey.Questions.Select(q => q.Id).ToHashSet();

            if (questionIds.Any())
            {
                if (!questionIds.All(id => surveyQuestionIds.Contains(id)))
                    return BadRequest("One or more question IDs are invalid for this survey.");
            }

            // Enforce required questions are present and answered
            var requiredQuestions = survey.Questions
                .Where(q => q.IsRequired)
                .Select(q => new { q.Id, q.QuestionType, q.QuestionText })
                .ToList();

            if (requiredQuestions.Any())
            {
                var answersDict = (input.Answers ?? Enumerable.Empty<SubmitAnswerDto>())
                                    .ToDictionary(a => a.QuestionId, a => a);

                foreach (var rq in requiredQuestions)
                {
                    if (!answersDict.TryGetValue(rq.Id, out var provided))
                        return BadRequest($"Answer required for question id {rq.Id} ('{rq.QuestionText}').");

                    if (rq.QuestionType.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                    {
                        if (provided.RatingValue == null)
                            return BadRequest($"Answer required for rating question id {rq.Id} ('{rq.QuestionText}').");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(provided.AnswerText))
                            return BadRequest($"Answer required for question id {rq.Id} ('{rq.QuestionText}').");
                    }
                }
            }
            var response = _mapper.Map<SurveyResponse>(input);
            response.SurveyId = surveyId;
            response.SubmittedAt = DateTime.UtcNow;

            // ensure answers are sanitized
            if (response.Answers != null)
            {
                foreach (var a in response.Answers)
                {
                    a.AnswerText = string.IsNullOrWhiteSpace(a.AnswerText) ? null : a.AnswerText;
                }
            }

            var created = await _responseService.CreateAsync(response);

            var createdDto = _mapper.Map<SurveyResponseReadDto>(created);

            return CreatedAtAction(nameof(GetResponses), new { surveyId = surveyId }, createdDto);
        }

        // ---------- DELETE RESPONSE (Admin) ----------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResponse(int surveyId, int id)
        {
            var response = await _responseService.GetByIdAsync(id);

            if (response == null || response.SurveyId != surveyId) return NotFound();

            var deleted = await _responseService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
