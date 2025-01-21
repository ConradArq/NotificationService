using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Interfaces.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class EmailNotificationRepository : GenericRepository<EmailNotification>, IEmailNotificationRepository
    {
        public EmailNotificationRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
}
