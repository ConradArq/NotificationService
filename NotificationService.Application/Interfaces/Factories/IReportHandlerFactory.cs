using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Domain.Interfaces.Providers;

namespace NotificationService.Application.Interfaces.Factories
{
    public interface IReportHandlerFactory
    {
        IReportHandler GetHandler<T>();
    }
}
