using NotificationService.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Interfaces.Services
{
    public interface IEmailQueueService
    {
        void EnqueueEmail(EmailNotification emailNotification);
        bool TryDequeue(out EmailNotification emailNotification);
    }
}
