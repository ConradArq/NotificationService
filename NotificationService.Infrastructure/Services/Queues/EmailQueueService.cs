using NotificationService.Domain.Models.Entities;
using System.Collections.Concurrent;

namespace NotificationService.Infrastructure.Services.Queues
{
    public interface IEmailQueueService
    {
        void EnqueueEmail(EmailNotification emailNotification);
        bool TryDequeue(out EmailNotification emailNotification);
    }

    public class EmailQueueService : IEmailQueueService
    {
        private readonly ConcurrentQueue<EmailNotification> _emailQueue = new ConcurrentQueue<EmailNotification>();

        public void EnqueueEmail(EmailNotification emailNotification)
        {
            _emailQueue.Enqueue(emailNotification);
        }

        public bool TryDequeue(out EmailNotification emailNotification)
        {
            return _emailQueue.TryDequeue(out emailNotification!);
        }
    }
}
