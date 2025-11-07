using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.Models
{
    public class SurveyQuestion
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required, StringLength(300)]
        public string QuestionText { get; set; }

        [Required, StringLength(50)]
        public string QuestionType { get; set; } = "Text";
        // Can be: Text, Rating, MultipleChoice

        public string? Options { get; set; } // Comma-separated for multiple choice

        // Navigation
        public Survey? Survey { get; set; }
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}
