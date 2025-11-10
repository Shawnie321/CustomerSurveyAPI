namespace CustomerSurveyAPI.Models
{
    public class Survey
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
        public ICollection<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();
    }
}