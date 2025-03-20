﻿using NotificationService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification
{
    public class SearchNotificationDto
    {
        public string? UserId { get; set; }
        public string? RoleId { get; set; }


        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int? StatusId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
