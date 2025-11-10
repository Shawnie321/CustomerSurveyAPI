using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyQuestionReadDto
    {
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public string QuestionText { get; set; } = null!;

        public string QuestionType { get; set; } = "Text"; // Can be: Text, Rating, MultipleChoice

        public string? Options { get; set; } // Comma-separated for multiple choice

        public bool IsRequired { get; set; } = false;

        public int AnswerCount { get; set; } = 0;
    }
}
