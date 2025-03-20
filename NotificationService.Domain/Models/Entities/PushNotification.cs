using NotificationService.Shared.Attributes;

namespace NotificationService.Domain.Models.Entities
{
    [Discoverable]
    public class PushNotification: Notification
    {
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
