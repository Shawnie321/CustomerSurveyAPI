using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ISurveyResponseService _responseService;

        public UsersController(ISurveyResponseService responseService)
        {
            _responseService = responseService;
        }

        // GET /api/users/me/completed-surveys
        [Authorize]
        [HttpGet("me/completed-surveys")]
        public async Task<IActionResult> GetMyCompletedSurveys()
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized();

            var ids = await _responseService.GetCompletedSurveyIdsByUsernameAsync(username);
            return Ok(ids); // returns int[] by default: [1, 2, 3]
        }

        // Admin: GET /api/users/{username}/completed-surveys
        [Authorize(Roles = "Admin")]
        [HttpGet("{username}/completed-surveys")]
        public async Task<IActionResult> GetUserCompletedSurveys(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");

            var ids = await _responseService.GetCompletedSurveyIdsByUsernameAsync(username);
            return Ok(ids);
        }
    }
}