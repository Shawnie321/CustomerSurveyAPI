using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyResponseUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = null!;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
