using NotificationService.Application.Attributes;
using System.ComponentModel;

namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class CreateEmailTemplateDto
    {
        [LocalizedStringLength(100)]
        [LocalizedRequired]
        public string Name { get; set; } = string.Empty;

        [LocalizedStringLength(100)]
        [LocalizedRequired]
        public string Subject { get; set; } = string.Empty;

        [LocalizedStringLength(10000)]
        [LocalizedRequired]
        public string Body { get; set; } = string.Empty;

        [EnumValueValidation(typeof(Domain.Enums.Status))]
        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int StatusId { get; set; } = (int)Domain.Enums.Status.Active;
    }
}