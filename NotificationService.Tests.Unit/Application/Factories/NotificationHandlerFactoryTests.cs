using Moq;
using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Application.Factories;
using NotificationService.Application.Interfaces.Handlers;

namespace NotificationService.Tests.Unit.Application.Factories
{
    public class NotificationHandlerFactoryTests
    {
        [Test]
        public void GetHandler_Should_Resolve_Correct_Handler()
        {
            var emailNotificationHandler = new Mock<INotificationHandler<CreateEmailNotificationDto>>();

            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
            .Setup(sp => sp.GetService(typeof(INotificationHandler<CreateEmailNotificationDto>)))
            .Returns(emailNotificationHandler.Object);

            var handlerFactory = new NotificationHandlerFactory(mockServiceProvider.Object);

            var handler = handlerFactory.GetHandler(typeof(CreateEmailNotificationDto));

            Assert.NotNull(handler);
            Assert.That(handler, Is.InstanceOf<INotificationHandler<CreateEmailNotificationDto>>(), "Factory did not resolve the correct handler type");
        }
    }
}
