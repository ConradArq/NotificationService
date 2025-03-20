using NotificationService.Application.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.SmtpConfig
{
    public class UpdateSmtpConfigDto
    {
        [LocalizedStringLength(255)]
        public string? Server { get; set; }

        [LocalizedRange(1, 65535)]
        [DefaultValue(587)]
        public int? Port { get; set; }

        [LocalizedEmailAddress]
        [DefaultValue("example@example.com")]
        public string? FromAddress { get; set; }

        public string? FromName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        [DefaultValue(true)]
        public bool? EnableSsl { get; set; }

        [EnumValueValidation(typeof(Domain.Enums.Status))]
        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int? StatusId { get; set; }
    }
}
