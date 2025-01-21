using NotificationService.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Models.Entities
{
    [Discoverable]
    public class PushNotification: Notification
    {
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
