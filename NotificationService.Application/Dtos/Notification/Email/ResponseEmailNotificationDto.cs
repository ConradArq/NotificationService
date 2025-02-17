using Microsoft.AspNetCore.Http;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification.Email
{
    public class ResponseEmailNotificationDto: ResponseNotificationDto
    {
        public int? TemplateId { get; set; }
        public object? TemplatePlaceholderMappings { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<string>? To { get; set; }
        public List<string>? CC { get; set; }
        public List<string>? BCC { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
