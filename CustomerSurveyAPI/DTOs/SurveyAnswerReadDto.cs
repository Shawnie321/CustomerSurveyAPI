using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyAnswerReadDto
    {
        public int Id { get; set; }

        [ForeignKey("SurveyResponse")]
        public int SurveyResponseId { get; set; }

        [ForeignKey("SurveyQuestion")]
        public int QuestionId { get; set; }

        public string? AnswerText { get; set; }

        public int? RatingValue { get; set; }
    }
}
