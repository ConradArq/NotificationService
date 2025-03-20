using NotificationService.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Application.Attributes
{
    /// <summary>
    /// A custom validation attribute to ensure that at least one recipient is provided for an email notification.
    /// This attribute checks if one or more of the following properties are set on the validated object:
    /// <c>UserId</c>, <c>RoleId</c>, or <c>To</c>.
    /// </summary>
    /// <remarks>
    /// - The validation will pass if at least one of the following conditions is met:
    ///   - <c>UserId</c> is not null or empty.
    ///   - <c>RoleId</c> is not null or empty.
    ///   - <c>To</c> is a non-empty list of email addresses.
    /// - If none of these conditions are met, the validation fails with an error message.
    /// </remarks>
    public class EmailNotificationRecipientRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;

            var userId = instance?.GetType().GetProperty("UserId")?.GetValue(instance) as string;
            var roleId = instance?.GetType().GetProperty("RoleId")?.GetValue(instance) as string;
            var to = instance?.GetType().GetProperty("To")?.GetValue(instance) as List<string>;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(roleId) && (to == null || to.Count == 0))
            {
                return new ValidationResult(ValidationMessages.RequiredEmailNotificationRecipientError);
            }

            return ValidationResult.Success;
        }
    }
}
