using System.ComponentModel.DataAnnotations;

namespace CustomerSurveyAPI.Services
{
    public class DateOnlyRangeAttribute : ValidationAttribute
    {
        public DateOnly Minimum { get; }
        public DateOnly Maximum { get; }

        public DateOnlyRangeAttribute(string minimum, string maximum)
        {
            Minimum = DateOnly.Parse(minimum);

            if (maximum == "TodayMinus16")
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                Maximum = today.AddYears(-16);
            }
            else
            {
                Maximum = DateOnly.Parse(maximum);
            }
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateOnly date)
                return true;

            return date >= Minimum && date <= Maximum;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be atleast 16 years or older.";
        }
    }
}
