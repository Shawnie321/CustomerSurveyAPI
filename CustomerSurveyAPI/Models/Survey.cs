using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSurveyAPI.Models
{
    public class Survey
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SurveyQuestion> Questions { get; set; } = new List<SurveyQuestion>();
        public ICollection<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();
    }
}