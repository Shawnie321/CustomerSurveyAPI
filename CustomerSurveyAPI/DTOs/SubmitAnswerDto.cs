namespace CustomerSurveyAPI.DTOs
{
    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? RatingValue { get; set; }
    }
}
