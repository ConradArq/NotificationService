using NotificationService.Domain.Models;
using System.Net;

namespace NotificationService.Infrastructure.SignalR.Models
{
    public class SignalRNotificationResponseDto
    {
        public string? Message { get; set; }
        public Notification? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}
