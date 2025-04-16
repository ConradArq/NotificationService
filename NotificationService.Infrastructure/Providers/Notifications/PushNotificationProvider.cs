using NotificationService.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Interfaces.Providers;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Infrastructure.SignalR.Hubs;
using NotificationService.Infrastructure.SignalR.Models;
using NotificationService.Shared.Resources;
using NotificationService.Infrastructure.Interfaces.Providers;
using NotificationService.Domain.Enums;

namespace NotificationService.Infrastructure.Providers.Notifications
{
    public class PushNotificationProvider : INotificationProvider
    {
        public NotificationType NotificationType => NotificationType.Push;

        private readonly IHttpService _httpService;
        private readonly ILogger<PushNotificationProvider> _logger;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PushNotificationProvider(IHttpService httpService, ILogger<PushNotificationProvider> logger, IConfiguration configuration, IJwtTokenProvider jwtTokenProvider, IHubContext<NotificationHub> hubContext)
        {
            _httpService = httpService;
            _logger = logger;
            _configuration = configuration;
            _jwtTokenProvider = jwtTokenProvider;
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            if (notification is not PushNotification pushNotification)
            {
                throw new ArgumentException("Invalid notification type for push provider.");
            }

            // Uncomment to send notification to a specific endpoint that handles notifications. As it stands now, notifications are sent to SignalR hub.

            ////var clientAppBaseUrl = _configuration["ExternalApis:ClientApp:BaseUrl"];
            ////var sendNotificationEndpoint = _configuration["ExternalApis:ClientApp:Endpoints:SendNotification"];
            ////var sendNotificationFullUrl = string.Concat(clientAppBaseUrl, sendNotificationEndpoint);

            ////var headers = new Dictionary<string, string>();

            ////var userAuthenticationToken = _jwtTokenProvider.GetUserAuthenticationToken();

            ////if (!string.IsNullOrEmpty(userAuthenticationToken))
            ////{
            ////    headers.Add("Authorization", string.Concat("Bearer ", _jwtTokenProvider.GetUserAuthenticationToken()));
            ////}

            ////_ = await _httpService.SendAsync<string>(HttpMethod.Post, sendNotificationFullUrl, pushNotification, contentType: "application/json", headers);

            SignalRNotificationResponseDto signalRNotificationResponseDto = new();
            signalRNotificationResponseDto.Data = pushNotification;
            signalRNotificationResponseDto.Message = string.Format(GeneralMessages.PushNotificationSentMessage, pushNotification.Message, string.IsNullOrEmpty(pushNotification.UserId) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : pushNotification.UserId, string.IsNullOrEmpty(pushNotification.RoleId) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : pushNotification.RoleId);

            await _hubContext.Clients.All.SendAsync(_configuration["SignalR:Events:ReceiveNotification"] ?? "ReceiveNotification", pushNotification);

            _logger.LogInformation(signalRNotificationResponseDto.Message);
        }        
    }
}