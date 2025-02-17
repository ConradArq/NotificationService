using Microsoft.AspNetCore.Http;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Application.Attributes;

namespace NotificationService.Application.Dtos.Notification.Email
{
    [EmailTemplateValidation]
    [EmailNotificationRecipientRequired]
    public class CreateEmailNotificationDto : CreateNotificationDto
    {
        [GreaterThanZero]
        [DefaultValue(1)]
        public int? TemplateId { get; set; }
        public string? TemplatePlaceholderMappings { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        [EmailList]
        [DefaultValue(new[] { "example@example.com" })]
        public List<string>? To { get; set; }

        [EmailList]
        [DefaultValue(new[] { "example@example.com" })]
        public List<string>? CC { get; set; }

        [EmailList]
        [DefaultValue(new[] { "example@example.com" })]
        public List<string>? BCC { get; set; }

        public List<IFormFile>? FileAttachments { get; set; }
    }
}
