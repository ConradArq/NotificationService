using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Providers
{
    /// <summary>
    /// Defines a contract for notification providers that send notifications based on a specific type and role-based criteria.
    /// </summary>
    public interface INotificationProvider
    {
        /// <summary>
        /// Gets the type of notification that this provider is responsible for handling.
        /// </summary>
        NotificationType NotificationType { get; }

        /// <summary>
        /// Determines whether this provider can handle sending notifications for a specific role.
        /// If not overridden, defaults to true (meaning all roles are accepted).
        /// </summary>
        bool CanHandle(string? roleId) => true;

        /// <summary>
        /// Executes the actual logic for sending the notification.
        /// </summary>
        /// <param name="notification">The notification to be sent.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendNotificationAsync(Notification notification);
    }
}
