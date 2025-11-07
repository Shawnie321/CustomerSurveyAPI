using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.Models
{
    public class SurveyAnswer
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

        public SurveyResponse? SurveyResponse { get; set; }
        public SurveyQuestion? SurveyQuestion { get; set; }
    }
}