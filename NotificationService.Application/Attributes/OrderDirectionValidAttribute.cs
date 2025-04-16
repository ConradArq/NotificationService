using NotificationService.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Attributes
{
    /// <summary>
    /// Validates that the order direction is either "asc", "desc", or null/empty.
    /// </summary>
    public class OrderDirectionValidAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is string direction)
            {
                if (string.IsNullOrWhiteSpace(direction) || direction == "asc" || direction == "desc")
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ValidationMessages.InvalidOrderDirection);
        }
    }
}
