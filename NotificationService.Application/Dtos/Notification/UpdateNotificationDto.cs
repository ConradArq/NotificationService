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
    public class UpdateNotificationDto
    {
        [EnumValueValidation(typeof(Domain.Enums.Status))]
        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int? StatusId { get; set; } = (int)Domain.Enums.Status.Active;
    }
}
