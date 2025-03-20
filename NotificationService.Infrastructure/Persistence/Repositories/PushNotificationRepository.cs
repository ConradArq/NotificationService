using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class PushNotificationRepository : GenericRepository<PushNotification>, IPushNotificationRepository
    {
        public PushNotificationRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
}
