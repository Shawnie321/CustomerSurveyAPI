using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Models;
using CustomerSurveyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerSurveyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        // ---------- USER REGISTRATION ----------
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto userRequest)
        {
            if (await _userService.GetByUsernameAsync(userRequest.Username) != null)
                return BadRequest("Username already exists");

            var user = new User
            {
                FirstName = userRequest.FirstName,
                MiddleName = userRequest.MiddleName,
                LastName = userRequest.LastName,
                Username = userRequest.Username,
                Email = userRequest.Email,
                PhoneNumber = userRequest.PhoneNumber,
                DateOfBirth = userRequest.DateOfBirth,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRequest.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);

            return Ok("User registered successfully");
        }

        // ---------- ADMIN REGISTRATION ----------
        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(UserCreateDto adminRequest)
        {
            if (await _userService.GetByUsernameAsync(adminRequest.Username) != null)
                return BadRequest("Username already exists");

            var admin = new User
            {
                Username = adminRequest.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminRequest.Password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(admin);

            return Ok("Admin registered successfully");
        }

        // ---------- LOGIN ----------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var user = await _userService.GetByUsernameAsync(loginRequest.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var token = GenerateJwtToken(user);

            return Ok(new { token, username = user.Username, role = user.Role });
        }

        // ---------- TOKEN CREATION ----------
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var keyString = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT Key is not configured.");

            var key = Encoding.UTF8.GetBytes(keyString);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}