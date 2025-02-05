using NotificationService.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class EmailNotificationRepository : GenericRepository<EmailNotification>, IEmailNotificationRepository
    {
        public EmailNotificationRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
}
