using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Providers;

namespace NotificationService.Application.Interfaces.Factories
{
    public interface INotificationProviderFactory
    {
        INotificationProvider Create(NotificationType notificationType, string? roleId = null);
    }
}
