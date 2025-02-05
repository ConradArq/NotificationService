using NotificationService.Application.Dtos.Notification.Push;
using NotificationService.Domain.Interfaces.Providers;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using AutoMapper;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Handlers
{
    public class PushNotificationHandler : INotificationHandler<CreatePushNotificationDto>
    {
        private readonly INotificationProvider _pushProvider;
        private readonly IMapper _mapper;


        public PushNotificationHandler(INotificationProviderFactory notificationProviderFactory, IMapper mapper)
        {
            _pushProvider = notificationProviderFactory.Create(NotificationType.Push);
            _mapper = mapper;
        }

        public async Task<Notification> HandleAsync(CreatePushNotificationDto createPushNotificationDto)
        {
            var pushNotification = _mapper.Map<PushNotification>(createPushNotificationDto);

            await _pushProvider.SendNotificationAsync(pushNotification);

            return pushNotification;
        }
    }
}
