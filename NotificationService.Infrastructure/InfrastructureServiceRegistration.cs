using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces.Providers;
using NotificationService.Infrastructure.Interfaces.Services;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;
using NotificationService.Infrastructure.Providers.Notifications;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Services.Queues;
using NotificationService.Infrastructure.Services.BackgroundServices;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using NotificationService.Infrastructure.Quartz;
using NotificationService.Infrastructure.Quartz.Jobs;
using NotificationService.Infrastructure.Providers;
using NotificationService.Infrastructure.Interfaces.Providers;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<QuartzSettings>(configuration.GetSection("QuartzSettings"));

            services.AddHttpClient();

            services.AddSignalR();

            services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
            services.AddTransient<INotificationProvider, EmailNotificationProvider>();
            services.AddTransient<INotificationProvider, PushNotificationProvider>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
            services.AddScoped<IPushNotificationRepository, PushNotificationRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<ISmtpConfigRepository, SmtpConfigRepository>();
            services.AddScoped<IExternalRepository, ExternalRepository>();

            services.AddSingleton<IEmailQueueService, EmailQueueService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IHttpService, HttpService>();

            services.AddHostedService<EmailBackgroundService>();

            // Uncomment the following to use an in-memory store for Quartz (not recommended for production/distributed setups)
            ////services.AddQuartz(q =>
            ////{
            ////    q.UseSimpleTypeLoader();
            ////    q.UseInMemoryStore();
            ////    q.UseDefaultThreadPool(tp =>
            ////    {
            ////        tp.MaxConcurrency = 10;
            ////    });
            ////});

            // Configure Quartz to use a database for job persistence and distributed locking
            services.AddQuartz(q =>
            {
                q.UseSimpleTypeLoader();
                q.UsePersistentStore(store =>
                {
                    store.UseProperties = true;
                    store.UseSqlServer(sqlOptions =>
                    {
                        var connectionString = configuration.GetConnectionString("ConnectionString")
                            ?? throw new InvalidOperationException("Missing connection string: 'ConnectionString' in configuration.");

                        sqlOptions.ConnectionString = connectionString;
                    });
                    store.UseNewtonsoftJsonSerializer();
                });
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<QuartzScheduler>();
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<NotificationCleanUpJob>();

            services.AddDbContext<NotificationServiceDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
                options.EnableSensitiveDataLogging();
                ////*,sqlServerOptions => sqlServerOptions.CommandTimeout(60)*/)
            });

            services.AddDbContext<ExternalDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
                options.EnableSensitiveDataLogging();
            });

            // Uncomment to use in memory database
            ////services.AddDbContext<NotificationServiceDbContext>(options =>
            ////{
            ////    options.UseInMemoryDatabase("NotificationServiceDb");
            ////});

            ////services.AddDbContext<ExternalDbContext>(options =>
            ////{
            ////    options.UseInMemoryDatabase("ExternalDb");
            ////});

            return services;
        }
    }
}
