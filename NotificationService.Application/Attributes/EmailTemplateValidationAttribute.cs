using NotificationService.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Application.Attributes
{
    /// <summary>
    /// A custom validation attribute to ensure that an email template includes either a subject and body or a template ID.
    /// This attribute validates that if the <c>Subject</c> or <c>Body</c> properties are not provided,
    /// the <c>TemplateId</c> must be specified.
    /// </summary>
    /// <remarks>
    /// The validation passes if:
    /// - Both <c>Subject</c> and <c>Body</c> are provided.
    /// - Or, <c>TemplateId</c> is not null.
    /// </remarks>
    public class EmailTemplateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;

            var subject = instance?.GetType().GetProperty("Subject")?.GetValue(instance) as string;
            var body = instance?.GetType().GetProperty("Body")?.GetValue(instance) as string;
            var TemplateId = instance?.GetType().GetProperty("TemplateId")?.GetValue(instance) as int?;

            if ((string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body)) && TemplateId == null)
            {
                return new ValidationResult(ValidationMessages.InvalidEmailTemplateError);
            }

            return ValidationResult.Success;
        }
    }
}
