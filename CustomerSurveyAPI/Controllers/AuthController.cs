using Microsoft.AspNetCore.Mvc;
using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ---------- USER REGISTRATION ----------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userRequest)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userRequest.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = userRequest.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRequest.PasswordHash),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // ---------- ADMIN REGISTRATION ----------
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] User adminRequest)
        {
            if (await _context.Users.AnyAsync(u => u.Username == adminRequest.Username))
                return BadRequest("Username already exists");

            var admin = new User
            {
                Username = adminRequest.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminRequest.PasswordHash),
                Role = "Admin"
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("Admin registered successfully");
        }

        // ---------- LOGIN ----------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.PasswordHash, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var token = GenerateJwtToken(user);

            return Ok(new { token, username = user.Username, role = user.Role });
        }

        // ---------- TOKEN CREATION ----------
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

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