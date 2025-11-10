using System.Collections.Generic;

namespace CustomerSurveyAPI.DTOs
{
    public class SubmitResponseDto
    {
        public string Username { get; set; } = "Anonymous";
        public List<SubmitAnswerDto>? Answers { get; set; }
    }
}
