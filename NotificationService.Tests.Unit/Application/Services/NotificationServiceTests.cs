using AutoMapper;
using Moq;
using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Application.Dtos;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Services;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Tests.Unit.Application.Services
{
    public class NotificationServiceTests
    {
        [Test]
        public async Task NotifyUsersAsync_Should_Call_Dependencies_And_Return_Response()
        {
            // Arrange
            var requestDto = new CreateEmailNotificationDto();
            var emailNotification = new EmailNotification();
            var responseDto = new ResponseEmailNotificationDto();

            var mockMapper = new Mock<IMapper>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockNotificationHandler = new Mock<INotificationHandler<CreateEmailNotificationDto>>();
            var mockNotificationHandlerFactory = new Mock<INotificationHandlerFactory>();

            // Set up mocks
            mockMapper.Setup(m => m.Map<ResponseEmailNotificationDto>(emailNotification)).Returns(responseDto);

            mockNotificationHandler
                .Setup(h => h.HandleAsync(requestDto))
                .ReturnsAsync(emailNotification);

            mockUnitOfWork
                .Setup(u => u.Repository<EmailNotification>().Create(emailNotification))
                .Returns(emailNotification);

            mockNotificationHandlerFactory
                .Setup(f => f.GetHandler(typeof(CreateEmailNotificationDto)))
                .Returns(mockNotificationHandler.Object);

            var service = new NotificationService<EmailNotification, ResponseEmailNotificationDto>(
                mockMapper.Object,
                mockNotificationHandlerFactory.Object,
                mockUnitOfWork.Object
            );

            // Act
            var result = await service.NotifyUsersAsync(requestDto);

            // Assert
            Assert.NotNull(result);
            Assert.That(result, Is.TypeOf<ResponseDto<ResponseEmailNotificationDto>>());

            // Verify dependency interactions
            mockNotificationHandlerFactory.Verify(f => f.GetHandler(typeof(CreateEmailNotificationDto)), Times.Once);
            mockNotificationHandler.Verify(h => h.HandleAsync(requestDto), Times.Once);
            mockUnitOfWork.Verify(u => u.Repository<EmailNotification>().Create(emailNotification), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
