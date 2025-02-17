using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Domain.Enums;
using NotificationService.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Infrastructure.SignalR.Hubs;
using NotificationService.Infrastructure.SignalR.Models;
using Microsoft.Extensions.Configuration;
using NotificationService.Shared.Resources;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure.Services.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEmailQueueService _emailQueueService;

        public EmailBackgroundService(IServiceScopeFactory serviceScopeFactory, IEmailQueueService emailQueueService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _emailQueueService = emailQueueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailBackgroundService>>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    if (_emailQueueService.TryDequeue(out var emailNotification))
                    {
                        SignalRNotificationResponseDto signalRNotificationResponseDto = new();
                        signalRNotificationResponseDto.Errors = new();
                        var signalRMethod = configuration["SignalR:Events:ReceiveNotification"] ?? "ReceiveNotification";

                        try
                        {
                            var smtpConfig = (await unitOfWork.SmtpConfigRepository.GetAsync(x => x.StatusId == (int)Status.Active)).FirstOrDefault();

                            if (smtpConfig != null)
                            {
                                await emailService.SendEmailAsync(emailNotification, smtpConfig);

                                signalRNotificationResponseDto.Data = emailNotification;
                                signalRNotificationResponseDto.Message = string.Format(GeneralMessages.EmailSentMessage, emailNotification.Subject, string.IsNullOrEmpty(emailNotification.ToRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.ToRecipients, string.IsNullOrEmpty(emailNotification.CcRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.CcRecipients, string.IsNullOrEmpty(emailNotification.BccRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.BccRecipients);

                                await hubContext.Clients.All.SendAsync(signalRMethod, emailNotification);

                                logger.LogInformation(signalRNotificationResponseDto.Message);
                            }
                            else
                            {
                                signalRNotificationResponseDto.Message = "Error while sending email";
                                signalRNotificationResponseDto.Errors.Add(string.Format(GeneralMessages.ErrorWhileSendingEmailMessage, emailNotification.Subject, string.IsNullOrEmpty(emailNotification.ToRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.ToRecipients, string.IsNullOrEmpty(emailNotification.CcRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.CcRecipients, string.IsNullOrEmpty(emailNotification.BccRecipients) ? GeneralMessages.NoneAvailableMessage.TrimEnd('.') : emailNotification.BccRecipients));

                                await hubContext.Clients.All.SendAsync(signalRMethod, signalRNotificationResponseDto);

                                logger.LogError($"Error in: {this.GetType().FullName}. There is no active SMTP configuration in the SmtpConfig table.");
                            }
                        }
                        catch (Exception ex)
                        {
                            signalRNotificationResponseDto.Message = "Error while sending email";
                            signalRNotificationResponseDto.Errors.Add(string.Format(GeneralMessages.ErrorWhileSendingEmailMessage, emailNotification.Subject, string.IsNullOrEmpty(emailNotification.ToRecipients) ? "None" : emailNotification.ToRecipients, string.IsNullOrEmpty(emailNotification.CcRecipients) ? "None" : emailNotification.CcRecipients, string.IsNullOrEmpty(emailNotification.BccRecipients) ? "None" : emailNotification.BccRecipients));
                            
                            await hubContext.Clients.All.SendAsync(signalRMethod, signalRNotificationResponseDto);

                            logger.LogError(ex, "Error in email background service.");
                        }
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}