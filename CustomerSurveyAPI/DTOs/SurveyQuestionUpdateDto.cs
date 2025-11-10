using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyQuestionUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required, StringLength(300)]
        public string QuestionText { get; set; } = null!;

        [Required, StringLength(50)]
        public string QuestionType { get; set; } = "Text";

        public string? Options { get; set; }

        public bool IsRequired { get; set; } = false;
    }
}
