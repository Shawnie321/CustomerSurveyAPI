namespace CustomerSurveyAPI.DTOs
{
    public class SurveyReadDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public List<SurveyQuestionReadDto> Questions { get; set; } = new();
    }
}
