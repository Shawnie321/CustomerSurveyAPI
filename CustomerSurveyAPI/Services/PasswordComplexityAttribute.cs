using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CustomerSurveyAPI.Services
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
                return false;

            // At least one number and one special character
            bool hasNumber = Regex.IsMatch(password, @"\d");
            bool hasSpecial = Regex.IsMatch(password, @"[^\w\d\s]");

            return hasNumber && hasSpecial;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must contain at least one number and one special character.";
        }
    }
}
