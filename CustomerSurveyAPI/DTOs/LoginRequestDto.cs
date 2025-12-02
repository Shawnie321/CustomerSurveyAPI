namespace CustomerSurveyAPI.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; // plain password in request
    }
}
