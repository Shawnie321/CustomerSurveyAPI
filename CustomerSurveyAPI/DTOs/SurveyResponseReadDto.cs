using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyResponseReadDto
    {
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public string Username { get; set; } = null!;

        public DateTime SubmittedAt { get; set; }

        public bool ConsentGiven { get; set; }
    }
}
