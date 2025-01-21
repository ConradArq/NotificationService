using NotificationService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces.Infrastructure.Providers
{
    public interface INotificationProvider
    {
        Task SendNotificationAsync(Notification notification);
    }
}
