using NotificationService.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Application.Attributes
{
    /// <summary>
    /// A custom validation attribute for validating a list of email addresses.
    /// This attribute checks each email address in the provided list to ensure it is valid.
    /// If any email addresses are invalid, the validation fails, and a message indicating the invalid emails is returned.
    /// </summary>
    /// <remarks>
    /// This attribute uses the built-in <see cref="EmailAddressAttribute"/> to validate each email in the list.
    /// Empty or whitespace-only email strings are also considered invalid.
    /// If the value is null or not a list of strings, the validation passes by default.
    /// </remarks>
    public class EmailListAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var emailList = value as List<string>;
            if (emailList == null)
            {
                return ValidationResult.Success;
            }

            var invalidEmails = new List<string>();
            var emailAddressAttribute = new EmailAddressAttribute();

            foreach (var email in emailList)
            {
                if (string.IsNullOrWhiteSpace(email) || !emailAddressAttribute.IsValid(email))
                {
                    invalidEmails.Add(string.IsNullOrWhiteSpace(email) ? "(empty string)" : email);
                }
            }

            if (invalidEmails.Count > 0)
            {
                var errorMessage = string.Format(ValidationMessages.InvalidEmailListError, string.Join(", ", invalidEmails));
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
