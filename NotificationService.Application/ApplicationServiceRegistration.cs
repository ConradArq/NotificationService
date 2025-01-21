using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Application.Dtos.Notification.Push;
using NotificationService.Application.Factories;
using NotificationService.Application.Handlers;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Mappings;
using NotificationService.Application.Services;
using NotificationService.Domain.Models.Entities;
using System.Reflection;

namespace NotificationService.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationHandler<CreateEmailNotificationDto>, EmailNotificationHandler>();
            services.AddTransient<INotificationHandler<CreatePushNotificationDto>, PushNotificationHandler>();
            services.AddScoped<INotificationHandlerFactory, NotificationHandlerFactory>();
            services.AddScoped<IReportHandlerFactory, ReportHandlerFactory>();
            services.AddScoped<INotificationProviderFactory, NotificationProviderFactory>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<INotificationService<EmailNotification, ResponseEmailNotificationDto>, NotificationService<EmailNotification, ResponseEmailNotificationDto>>();
            services.AddScoped<INotificationService<PushNotification, ResponsePushNotificationDto>, NotificationService<PushNotification, ResponsePushNotificationDto>>();
            services.AddScoped<ISmtpConfigService, SmtpConfigService>();
            services.AddScoped<IReportService<EmailNotification>, ReportService<EmailNotification>>();
            services.AddScoped<IReportService<PushNotification>, ReportService<PushNotification>>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            QueryProfile.InitializeMappings();

            return services;
        }
    }
}
