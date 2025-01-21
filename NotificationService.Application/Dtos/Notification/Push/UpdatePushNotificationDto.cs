using Microsoft.AspNetCore.Http;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification.Push
{
    public class UpdatePushNotificationDto : UpdateNotificationDto
    {
        public bool? IsRead { get; set; }
    }
}
