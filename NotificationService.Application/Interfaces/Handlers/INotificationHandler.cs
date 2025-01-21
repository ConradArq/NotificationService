using NotificationService.Application.Dtos.Notification;
using NotificationService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces.Handlers
{
    public interface INotificationHandler<TDto> where TDto: CreateNotificationDto
    {
        Task<Notification> HandleAsync(TDto dto);
    }
}
