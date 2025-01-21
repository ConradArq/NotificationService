using NotificationService.Domain.Models.Entities;

namespace NotificationService.Infrastructure.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailNotification emailNotification, SmtpConfig smtpConfig);
    }
}
