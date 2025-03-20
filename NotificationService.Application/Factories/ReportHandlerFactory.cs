using Microsoft.Extensions.Configuration;
using NotificationService.Application.Handlers;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;

namespace NotificationService.Application.Factories
{
    public class ReportHandlerFactory : IReportHandlerFactory
    {
        private readonly Dictionary<Type, Func<IReportHandler>> _handlers;

        public ReportHandlerFactory(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _handlers = new()
            {
                { typeof(EmailNotification), () => new ReportEmailNotificationHandler(unitOfWork, configuration) },
                { typeof(PushNotification), () => new ReportPushNotificationHandler(unitOfWork, configuration) }
            };
        }

        public IReportHandler GetHandler<T>()
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                return _handlers[typeof(T)]();
            }

            throw new InvalidOperationException($"No handler found for type {typeof(T).Name}");
        }
    }
}
