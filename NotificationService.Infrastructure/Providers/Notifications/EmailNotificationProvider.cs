using NotificationService.Infrastructure.Services.Queues;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using NotificationService.Domain.Interfaces.Infrastructure.Providers;

namespace NotificationService.Infrastructure.Providers.Notifications
{
    public class EmailNotificationProvider : INotificationProvider
    {
        private readonly IEmailQueueService _emailQueueService;

        public EmailNotificationProvider(IEmailQueueService emailQueueService)
        {
            _emailQueueService = emailQueueService;
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