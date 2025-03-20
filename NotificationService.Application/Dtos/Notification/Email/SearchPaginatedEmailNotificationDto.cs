using Microsoft.AspNetCore.Http;
using NotificationService.Application.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification.Email
{
    public class SearchPaginatedEmailNotificationDto : SearchPaginatedNotificationDto
    {
        [DefaultValue(1)]
        public int? TemplateId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com" })]
        public List<string>? To
        {
            get => string.IsNullOrEmpty(ToRecipients)
                ? null
                : ToRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            set => ToRecipients = value == null ? null : string.Join(",", value);
        }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com" })]
        public List<string>? CC
        {
            get => string.IsNullOrEmpty(CcRecipients)
                ? null
                : CcRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            set => CcRecipients = value == null ? null : string.Join(",", value);
        }

        [IgnoreInQueryPredicate]
        [DefaultValue(new[] { "example@example.com" })]
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
