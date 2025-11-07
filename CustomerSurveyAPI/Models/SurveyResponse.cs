using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Survey? Survey { get; set; }
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}
