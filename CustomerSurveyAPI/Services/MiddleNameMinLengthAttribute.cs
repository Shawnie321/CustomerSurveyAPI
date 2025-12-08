using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.Services
{
    public class MiddleNameMinLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        public MiddleNameMinLengthAttribute(int minLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var str = value as string;
            if (string.IsNullOrEmpty(str))
                return ValidationResult.Success; // Allow null or empty

            if (str.Length < _minLength)
                return new ValidationResult($"Middle Name must be at least {_minLength} characters long.");

            return ValidationResult.Success;
        }
    }
}