
namespace NotificationService.Application.Interfaces.Factories
{
    public interface INotificationHandlerFactory
    {
        object GetHandler(Type requestType);
    }
}
