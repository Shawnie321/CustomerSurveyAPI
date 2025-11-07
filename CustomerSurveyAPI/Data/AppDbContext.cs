using Microsoft.EntityFrameworkCore;
using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }
    }
}
