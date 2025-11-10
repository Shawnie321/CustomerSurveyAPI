using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyAnswerCreateDto
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("SurveyResponse")]
        public int SurveyResponseId { get; set; }

        [Required]
        [ForeignKey("SurveyQuestion")]
        public int QuestionId { get; set; }

        [StringLength(500)]
        public string? AnswerText { get; set; }

        [Range(1, 10)]
        public int? RatingValue { get; set; }
    }
}
