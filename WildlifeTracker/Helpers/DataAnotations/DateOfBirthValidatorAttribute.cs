using System.ComponentModel.DataAnnotations;

namespace WildlifeTracker.Helpers.DataAnotations
{
    public class DateOfBirthValidatorAttribute : DateNotInFutureValidatorAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                if (date < DateOnly.FromDateTime(DateTime.Now.AddYears(-120)))
                {
                    return new ValidationResult("The date cannot be more than 120 years in the past.");
                }
                else if (date > DateOnly.FromDateTime(DateTime.Now.AddYears(-14)))
                {
                    return new ValidationResult("The user must be at least 14 years old.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
