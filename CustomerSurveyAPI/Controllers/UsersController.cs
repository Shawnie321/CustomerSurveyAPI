using AutoMapper;
using CustomerSurveyAPI.DTOs;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(
            ISurveyResponseService responseService,
            IUserService userService,
            IMapper mapper)
        {
            _responseService = responseService;
            _userService = userService;
            _mapper = mapper;
        }

        // Admin-only: GET /api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<UserReadDto>>(users);
            return Ok(result);
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