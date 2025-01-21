using NotificationService.Domain.Interfaces.Infrastructure.Providers;

namespace NotificationService.Application.Interfaces.Factories
{
    public interface INotificationProviderFactory
    {
        INotificationProvider Create(Type notificationType);
    }
}
