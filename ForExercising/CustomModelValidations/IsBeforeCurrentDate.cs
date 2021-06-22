using System;
using System.ComponentModel.DataAnnotations;

namespace ForExercising.CustomModelValidations
{
    public class IsBeforeCurrentDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value >= DateTime.UtcNow)
            {
                return new ValidationResult($"Year should be between 1900 and {DateTime.UtcNow.Year}");
            }

            return ValidationResult.Success;
        }
    }
}
