using NotificationService.Domain.Models.Entities;
using NotificationService.Infrastructure.Interfaces.Services;
using System.Threading.Channels;

namespace NotificationService.Infrastructure.Services.Queues
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly Channel<EmailNotification> _channel;

        public EmailQueueService()
        {
            // Enable email throttling by limiting the channel to 100 queued (pending) items.
            // Producers will wait if the buffer is full, preventing overload and enforcing backpressure.
            var options = new BoundedChannelOptions(capacity: 100)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<EmailNotification>(options);
        }

        public void EnqueueEmail(EmailNotification emailNotification)
        {
            _channel.Writer.TryWrite(emailNotification);
        }

        public async Task<EmailNotification> DequeueEmailAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
