using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Domain.Models
{
    /// <summary>
    /// Represents a base email notification model that holds information about 
    /// the notification and its recipients.
    /// This class is not mapped to the database and is intended to be inherited by specific notification types.
    /// </summary>
    [NotMapped]
    public class Notification : BaseDomainModel
    {
        /// <summary>
        /// The ID of the user to receive the notification. 
        /// Optional: Notifications can be sent to a user or role.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The ID of the role to receive the notification. 
        /// Optional: Notifications can be sent to a user or role.
        /// </summary>
        public string? RoleId { get; set; }
    }
}