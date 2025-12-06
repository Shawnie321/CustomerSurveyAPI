using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CustomerSurveyAPI.DTOs
{
    public class SurveyUpdateDto
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public List<SurveyQuestionUpdateDto>? Questions { get; set; }
    }
}
