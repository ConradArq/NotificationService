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
    /// Ensures that OrderDirection is empty if OrderBy is not provided.
    /// </summary>
    public class OrderDirectionRequiresOrderByAttribute : ValidationAttribute
    {
        private readonly string _orderByProperty;

        public OrderDirectionRequiresOrderByAttribute(string orderByProperty)
        {
            _orderByProperty = orderByProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var orderByProperty = validationContext.ObjectType.GetProperty(_orderByProperty);

            if (orderByProperty == null)
            {
                return new ValidationResult($"Property '{_orderByProperty}' not found.");
            }

            var orderByValue = orderByProperty.GetValue(validationContext.ObjectInstance) as string;
            var orderDirectionValue = value as string;

            if (string.IsNullOrWhiteSpace(orderByValue) && !string.IsNullOrWhiteSpace(orderDirectionValue))
            {
                return new ValidationResult(ValidationMessages.OrderDirectionWithoutOrderByError);
            }

            return ValidationResult.Success;
        }
    }
}
