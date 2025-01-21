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

namespace NotificationService.Application.Dtos.Notification.Push
{
    [PushNotificationRecipientRequired]
    public class CreatePushNotificationDto : CreateNotificationDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
