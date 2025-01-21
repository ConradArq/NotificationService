using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Services.Queues
{
    public interface ILogQueueService
    {
        void EnqueueLog(object logEntry);
        Task<object?> DequeueLogAsync(CancellationToken cancellationToken);
    }

    public class LogQueueService : ILogQueueService
    {
        private readonly Channel<object> _channel;

        public LogQueueService()
        {
            _channel = Channel.CreateUnbounded<object>();
        }

        public void EnqueueLog(object logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException(nameof(logEntry));
            _channel.Writer.TryWrite(logEntry);
        }

        public async Task<object?> DequeueLogAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
