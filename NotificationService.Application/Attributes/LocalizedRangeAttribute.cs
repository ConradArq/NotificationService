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
    /// A custom validation attribute for value ranges that supports localization.
    /// This attribute extends the built-in <see cref="RangeAttribute"/> to support resource-based error messages.
    /// It resolves the limitation that .NET's <see cref="RangeAttribute"/> embeds error messages into the 
    /// attribute assembly at compile time, which makes localization with resource files impossible.
    /// </summary>
    public class LocalizedRangeAttribute : ValidationAttribute
    {
        private readonly RangeAttribute _rangeAttribute;

        public LocalizedRangeAttribute(int minimum, int maximum)
        {
            _rangeAttribute = new RangeAttribute(minimum, maximum);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!_rangeAttribute.IsValid(value))
            {
                var errorMessage = string.Format(ValidationMessages.FieldRangeError, _rangeAttribute.Minimum, _rangeAttribute.Maximum);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
