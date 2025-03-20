using NotificationService.Application.Interfaces.Factories;
using NotificationService.Application.Interfaces.Handlers;

namespace NotificationService.Application.Factories
{
    public class NotificationHandlerFactory : INotificationHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetHandler(Type requestType)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(requestType);

            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for type {requestType.Name}");
            }

            return handler;
        }
    }
}
