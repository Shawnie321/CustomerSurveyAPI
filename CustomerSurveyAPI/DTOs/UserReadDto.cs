namespace CustomerSurveyAPI.DTOs
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; }
    }
}
