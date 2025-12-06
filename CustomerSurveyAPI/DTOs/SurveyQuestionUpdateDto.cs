using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyQuestionUpdateDto
    {
        public int? Id { get; set; }

        [Required, StringLength(300)]
        public string QuestionText { get; set; } = null!;

        [Required, StringLength(50)]
        public string QuestionType { get; set; } = "Text";

        public string? Options { get; set; }

        public bool IsRequired { get; set; } = false;
    }
}
