using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyUpdateDto
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
