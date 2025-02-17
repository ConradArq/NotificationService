using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using NotificationService.Domain.Interfaces.Providers;
using NotificationService.Domain.Enums;
using NotificationService.Infrastructure.Interfaces.Services;

namespace NotificationService.Infrastructure.Providers.Notifications
{
    public class EmailNotificationProvider : INotificationProvider
    {
        public NotificationType NotificationType => NotificationType.Email;

        private readonly IEmailQueueService _emailQueueService;

        public EmailNotificationProvider(IEmailQueueService emailQueueService)
        {
            _emailQueueService = emailQueueService;
        }

        public bool CanHandle(string? roleId)
        {
            if (roleId == null)
            {
                return true;
            }
            else
            {
                // Implement logic to determine if this provider is responsible for handling notifications for the given role.
                return true;
            }
        }

        public Task SendNotificationAsync(Notification notification)
        {
            if (notification is not EmailNotification emailNotification)
            {
                throw new ArgumentException("Invalid notification type for email provider.");
            }

            _emailQueueService.EnqueueEmail(emailNotification);

            return Task.CompletedTask;
        }        
    }
}