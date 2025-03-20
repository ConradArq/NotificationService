using Microsoft.AspNetCore.SignalR;
using NotificationService.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace NotificationService.Infrastructure.SignalR.Hubs
{
    /// <summary>
    /// The NotificationHub class is a SignalR hub that enables real-time communication 
    /// between the server and connected clients. This hub is primarily used for broadcasting 
    /// notifications to all connected clients.
    /// 
    /// Clients can invoke methods on the hub to send notifications or listen for broadcasted 
    /// notifications using predefined events.
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly string _receiveNotificationEventName;

        public NotificationHub(IConfiguration configuration)
        {
            _receiveNotificationEventName = configuration["SignalR:Events:ReceiveNotification"] ?? "ReceiveNotification";
        }

        /// <summary>
        /// Handles client requests to send a notification to the server. 
        /// This method is intended to be invoked by SignalR clients to send a Notification object to the server. 
        /// The server will then broadcast the notification to all connected clients using the configured event name.
        /// 
        /// This method is not meant to be used by the server. Servers should use the IHubContext<NotificationHub>
        /// to broadcast notifications directly to clients.
        /// Use this method only when clients need to trigger server-side broadcasts of notifications.
        /// </summary>
        /// <param name="signalRNotificationResponseDto">The Notification object to be broadcasted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendNotification(Notification signalRNotificationResponseDto)
        {
            // Broadcast the notification to all connected clients using the configured event name
            await Clients.All.SendAsync(_receiveNotificationEventName, signalRNotificationResponseDto);
        }
    }
}
