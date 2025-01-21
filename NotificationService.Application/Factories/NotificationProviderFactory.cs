using NotificationService.Application.Interfaces.Factories;
using NotificationService.Domain.Interfaces.Infrastructure.Providers;

namespace NotificationService.Application.Factories
{
    internal class NotificationProviderFactory : INotificationProviderFactory
    {
        private readonly IEnumerable<INotificationProvider> _providers;

        public NotificationProviderFactory(IEnumerable<INotificationProvider> providers)
        {
            _providers = providers;
        }

        public INotificationProvider Create(Type notificationType)
        {
            var provider = _providers.FirstOrDefault(p => p.GetType().Name.Contains(notificationType.Name.ToString()));

            if (provider == null)
            {
                throw new ArgumentException("Tipo de notificación no válida.");
            }

            return provider;
        }
    }
}
