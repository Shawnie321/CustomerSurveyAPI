using CustomerSurveyAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CustomerSurveyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Add services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

// --- Database (Supabase PostgreSQL) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register application services for dependency injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<ISurveyQuestionService, SurveyQuestionService>();
builder.Services.AddScoped<ISurveyResponseService, SurveyResponseService>();
builder.Services.AddScoped<ISurveyAnswerService, SurveyAnswerService>();

// --- JWT Authentication ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
string? jwtKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

// --- CORS for React frontend ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // your React dev URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- Middleware pipeline ---
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();  // must be before Authorization and Swagger
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Seed admin user
using (var scope = app.Services.CreateScope())
{
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

    var adminUser = cfg["AdminUser:Username"];
    var adminPass = cfg["AdminUser:Password"];
    if (!string.IsNullOrWhiteSpace(adminUser) && !string.IsNullOrWhiteSpace(adminPass))
    {
        var existing = await userService.GetByUsernameAsync(adminUser);
        if (existing == null)
        {
            var admin = new CustomerSurveyAPI.Models.User
            {
                Username = adminUser,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPass),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            await userService.CreateAsync(admin);
        }
    }
}

app.Run();
