using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyCreateDto
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SurveyQuestionCreateDto>? Questions { get; set; }
    }
}
