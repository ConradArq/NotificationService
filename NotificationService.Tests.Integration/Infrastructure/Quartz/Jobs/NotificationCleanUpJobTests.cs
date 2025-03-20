using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Interfaces.Logging;
using NotificationService.Infrastructure.Logging.Models;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;
using NotificationService.Infrastructure.Quartz;
using NotificationService.Infrastructure.Quartz.Jobs;
using NotificationService.Shared.Extensions;
using NotificationService.Tests.Integration.Helpers;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace NotificationService.Tests.Integration.Infrastructure.Quartz.Jobs
{
    public class NotificationCleanUpJobTests
    {
        private IServiceProvider _serviceProvider;
        private Mock<ILogger<NotificationCleanUpJob>> _loggerMock;
        private Mock<IApiLogger> _apiLoggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<NotificationCleanUpJob>>();
            _loggerMock
                .Setup(logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )).Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0];
                    var eventId = (EventId)invocation.Arguments[1];
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = invokeMethod!.Invoke(formatter, new[] { state, exception });

                    TestContext.WriteLine(logMessage);
                }));

            _apiLoggerMock = new Mock<IApiLogger>();
            _apiLoggerMock
                .Setup(logger => logger.LogInfo(It.IsAny<AuditLog>()))
                .Callback((AuditLog auditLog) =>
                {
                    TestContext.WriteLine($"Log Message: {auditLog.Message}, Event Type: {auditLog.EventType}, Request URL: {auditLog.RequestUrl}, Entity Name: {auditLog.EntityName}, Old Data: {auditLog.OldData}, New Data: {auditLog.NewData}");
                });
            _apiLoggerMock
                .Setup(logger => logger.LogError(It.IsAny<ErrorLog>()))
                .Callback((ErrorLog errorLog) =>
                {
                    TestContext.WriteLine($"Error Date: {errorLog.ErrorDate}, Exception Type: {errorLog.ExceptionType}, Exception Message: {errorLog.ExceptionMessage}, Stack Trace: {errorLog.ErrorStackTrace}");
                });

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()

                .AddDbContext<NotificationServiceDbContext>(options =>
                    options.UseInMemoryDatabase("NotificationServiceDbContext"))

                .AddDbContext<ExternalDbContext>(options =>
                    options.UseInMemoryDatabase("ExternalDbContext"))

                .Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
                .Configure<QuartzSettings>(configuration.GetSection("QuartzSettings"))

                .AddHttpContextAccessor()
                .AddSingleton(_loggerMock.Object)
                .AddSingleton(_apiLoggerMock.Object)
                .AddTransient<IEmailNotificationRepository, EmailNotificationRepository>()
                .AddTransient<IPushNotificationRepository, PushNotificationRepository>()
                .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
                .AddTransient<ISmtpConfigRepository, SmtpConfigRepository>()
                .AddTransient<IExternalRepository, ExternalRepository>()
                .AddTransient<IUnitOfWork, UnitOfWork>()

                .AddQuartz(q =>
                {
                    q.UseInMemoryStore();
                    q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true)
                .AddSingleton<ISchedulerFactory, StdSchedulerFactory>()
                .AddSingleton<TestQuartzScheduler>()
                .AddSingleton<IJobFactory, JobFactory>()
                .AddSingleton<NotificationCleanUpJob>()

                .BuildServiceProvider();

            _serviceProvider = serviceProvider;

            var testQuartzScheduler = serviceProvider.GetRequiredService<TestQuartzScheduler>();
            testQuartzScheduler.StartAsync().Wait();
            //
            testQuartzScheduler.AddTriggerListenerToScheduler(1);

            SeedAppDatabase();
        }

        private void SeedAppDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationServiceDbContext>();

            context.Set<EmailNotification>().Add(new EmailNotification
            {
                Id = 1,
                CreatedDate = DateTime.Now.InTimeZone()
            });

            context.Set<EmailNotification>().Add(new EmailNotification
            {
                Id = 2,
                CreatedDate = DateTime.Now.InTimeZone().AddSeconds(-10)
            });

            context.Set<PushNotification>().Add(new PushNotification
            {
                Id = 1,
                CreatedDate = DateTime.Now.InTimeZone()
            });

            context.Set<PushNotification>().Add(new PushNotification
            {
                Id = 2,
                CreatedDate = DateTime.Now.InTimeZone().AddSeconds(-10)
            });

            context.SaveChanges();
        }

        [Test]
        public async Task NotificationCleanUpJob_ShouldRemoveNotificationFromDb_WhenScheduledAndTriggered()
        {
            var quartzSettings = _serviceProvider.GetRequiredService<IOptions<QuartzSettings>>();
            var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            var testQuartzScheduler = _serviceProvider.GetRequiredService<TestQuartzScheduler>();
            TimeSpan? jobRunningInterval = null;

            if (quartzSettings.Value.Jobs.NotificationCleanup.JobRunningInterval.Value > 0)
            {
                jobRunningInterval = QuartzSettings.GetTimeSpanFromInterval(quartzSettings.Value.Jobs.NotificationCleanup.JobRunningInterval);
            }

            await testQuartzScheduler.ScheduleNotificationsCleanUp(jobRunningInterval);

            await testQuartzScheduler.JobCompletion;

            var emailNotificationWithId1 = await unitOfWork.EmailNotificationRepository.GetSingleAsync(1);
            var emailNotificationWithId2 = await unitOfWork.EmailNotificationRepository.GetSingleAsync(2);
            var pushNotificationWithId1 = await unitOfWork.PushNotificationRepository.GetSingleAsync(1);
            var pushNotificationWithId2 = await unitOfWork.PushNotificationRepository.GetSingleAsync(2);

            Assert.That(emailNotificationWithId1, Is.Not.EqualTo(null), $"Email notification with Id '1' was deleted");
            Assert.That(emailNotificationWithId2, Is.EqualTo(null), $"Error deleting email notification with Id '2'");
            Assert.That(pushNotificationWithId1, Is.Not.EqualTo(null), $"Push notification with Id '1' was deleted");
            Assert.That(pushNotificationWithId2, Is.EqualTo(null), $"Error deleting push notification with Id '2'");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var testQuartzScheduler = _serviceProvider.GetRequiredService<TestQuartzScheduler>();
            testQuartzScheduler.ShutdownAsync().Wait();

            if (_serviceProvider is IDisposable disposableProvider)
            {
                disposableProvider.Dispose();
            }
        }

    }
}
