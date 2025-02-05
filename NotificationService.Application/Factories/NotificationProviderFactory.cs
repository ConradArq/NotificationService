using NotificationService.Application.Interfaces.Factories;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Providers;

namespace NotificationService.Application.Factories
{
    public class NotificationProviderFactory : INotificationProviderFactory
    {
        private readonly IEnumerable<INotificationProvider> _providers;

        public NotificationProviderFactory(IEnumerable<INotificationProvider> providers)
        {
            _providers = providers;
        }

        public INotificationProvider Create(NotificationType notificationType, string? roleId = null)
        {
            var provider = _providers.FirstOrDefault(p => p.NotificationType == notificationType && p.CanHandle(roleId));

            if (provider == null)
            {
                throw new InvalidOperationException("No suitable provider found.");
            }

            return provider;
        }
    }
}