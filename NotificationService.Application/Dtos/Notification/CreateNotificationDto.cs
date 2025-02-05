using NotificationService.Application.Attributes;
using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification
{
    public class CreateNotificationDto
    {
        /// <summary>
        /// The ID of the user who receives the notification. Optional, notifications can also be sent by RoleId 
        /// or email list if extended to support that.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The ID of the role who receives the notification. Optional, notifications can also be sent by UserId
        /// or email list if extended to support that.
        /// </summary>
        public string? RoleId { get; set; }
    }
}
