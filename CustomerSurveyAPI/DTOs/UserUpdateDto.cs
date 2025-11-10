using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
