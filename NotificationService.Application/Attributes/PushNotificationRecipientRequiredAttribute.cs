using NotificationService.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Application.Attributes
{
    /// <summary>
    /// A custom validation attribute to ensure that at least one recipient is provided for a push notification.
    /// This attribute checks if one or more of the following properties are set on the validated object:
    /// <c>UserId</c> or <c>RoleId</c>.
    /// </summary>
    /// <remarks>
    /// The validation passes if at least one of the following conditions is met:
    /// - <c>UserId</c> is not null or empty.
    /// - <c>RoleId</c> is not null or empty.
    /// If none of these conditions are met, the validation fails with an error message.
    /// </remarks>
    public class PushNotificationRecipientRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;

            var userId = instance?.GetType().GetProperty("UserId")?.GetValue(instance) as string;
            var roleId = instance?.GetType().GetProperty("RoleId")?.GetValue(instance) as string;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(roleId))
            {
                return new ValidationResult(ValidationMessages.RequiredPushNotificationRecipientError);
            }

            return ValidationResult.Success;
        }
    }
}
