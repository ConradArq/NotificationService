using Microsoft.AspNetCore.Http;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using NotificationService.Application.Attributes;

namespace NotificationService.Application.Dtos.Notification.Email
{
    public class SearchEmailNotificationDto : SearchNotificationDto
    {
        [DefaultValue(1)]
        public int? TemplateId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com"})]
        public List<string>? To
        {
            get => string.IsNullOrEmpty(ToRecipients)
                ? null
                : ToRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            set => ToRecipients = value == null ? null : string.Join(",", value);
        }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com"})]
        public List<string>? CC
        {
            get => string.IsNullOrEmpty(CcRecipients)
                ? null
                : CcRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            set => CcRecipients = value == null ? null : string.Join(",", value);
        }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com"})]
        public List<string>? BCC
        {
            get => string.IsNullOrEmpty(BccRecipients)
                ? null
                : BccRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            set => BccRecipients = value == null ? null : string.Join(",", value);
        }

        [JsonIgnore]
        public string? ToRecipients { get; set; }
        [JsonIgnore]
        public string? CcRecipients { get; set; }
        [JsonIgnore]
        public string? BccRecipients { get; set; }
    }
}
