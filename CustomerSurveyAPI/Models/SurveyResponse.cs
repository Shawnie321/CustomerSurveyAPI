using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public string Username { get; set; } = null!;

        public DateTime SubmittedAt { get; set; }

        // Navigation
        public Survey? Survey { get; set; }
        public bool ConsentGiven { get; set; }
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}
