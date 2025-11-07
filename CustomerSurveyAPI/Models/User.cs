using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        // store only the hash, never the plain password
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
