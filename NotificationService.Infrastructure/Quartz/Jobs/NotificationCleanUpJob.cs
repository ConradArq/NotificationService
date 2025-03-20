using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Services.BackgroundServices;
using NotificationService.Shared.Extensions;
using Quartz;

namespace NotificationService.Infrastructure.Quartz.Jobs
{
    public class NotificationCleanUpJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationCleanUpJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationCleanUpJob>>();
                var config = scope.ServiceProvider.GetRequiredService<IOptions<QuartzSettings>>();

                var notificationRetentionPeriod = QuartzSettings.GetTimeSpanFromInterval(config.Value.Jobs.NotificationCleanup.NotificationRetentionPeriod);
                var retentionThreshold = DateTime.Now.InTimeZone().Subtract(notificationRetentionPeriod);

                var emailNotifications = (await unitOfWork.EmailNotificationRepository.GetAsync(x => x.CreatedDate < retentionThreshold)).ToList(); 
                var pushNotifications = (await unitOfWork.PushNotificationRepository.GetAsync(x => x.CreatedDate < retentionThreshold)).ToList();

                unitOfWork.EmailNotificationRepository.DeleteRange(emailNotifications);
                unitOfWork.PushNotificationRepository.DeleteRange(pushNotifications);
                await unitOfWork.SaveAsync();

                logger.LogInformation($"Clean-up task performed. Email notification IDs deleted: {string.Join(", ", emailNotifications.Select(x => x.Id))}. Push notification IDs deleted: {string.Join(", ", pushNotifications.Select(x => x.Id))}.");
            }
        }
    }
}
