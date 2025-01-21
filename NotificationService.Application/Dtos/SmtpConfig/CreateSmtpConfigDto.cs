using NotificationService.Application.Attributes;
using System.ComponentModel;

namespace NotificationService.Application.Dtos.SmtpConfig
{
    public class CreateSmtpConfigDto
    {
        [LocalizedStringLength(255)]
        [LocalizedRequired]
        public string Server { get; set; } = string.Empty;

        [LocalizedRange(1, 65535)]
        [DefaultValue(587)]
        public int Port { get; set; }

        [LocalizedEmailAddress]
        [DefaultValue("example@example.com" )]
        public string FromAddress { get; set; } = string.Empty;

        public string FromName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool EnableSsl { get; set; } = true;

        [EnumValueValidation(typeof(Domain.Enums.Status))]
        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int StatusId { get; set; } = (int)Domain.Enums.Status.Active;
    }
}