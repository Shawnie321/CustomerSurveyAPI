using CustomerSurveyAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.DTOs
{
    public class UserUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [MinLength(3, ErrorMessage = "First Name must be at least 3 characters long")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(3, ErrorMessage = "Last Name must be at least 3 characters long")]
        public string LastName { get; set; } = null!;

        [MiddleNameMinLength(3)]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DateOnlyRangeAttribute("1900-01-01", "TodayMinus16", ErrorMessage = "You must be at least 16 years old to register.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format")]
        public string Email { get; set; } = null!;

        [MinLength(10, ErrorMessage = "Phone Number must be at least 10 digits long")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(6, ErrorMessage = "Username must be at least 6 characters long")]
        public string Username { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
