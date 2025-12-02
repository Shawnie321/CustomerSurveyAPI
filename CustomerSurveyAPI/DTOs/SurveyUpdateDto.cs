using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyUpdateDto
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
