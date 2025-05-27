using System.ComponentModel.DataAnnotations;

namespace WildlifeTracker.Helpers.DataAnotations
{
    public class DateNotInFutureValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                if (date > DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ValidationResult("The date cannot be a future date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
