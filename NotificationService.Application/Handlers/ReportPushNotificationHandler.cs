using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Services;
using Microsoft.Extensions.Configuration;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Handlers
{
    public class ReportPushNotificationHandler: IReportHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public ReportPushNotificationHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> HandleAsync() 
        {
            var entities = await _unitOfWork.PushNotificationRepository.GetAsync();
            var htmlRenderService = new HtmlRenderService();
            var templatePath = _configuration["Paths:NotificationsReportTemplate"];

            if (string.IsNullOrWhiteSpace(templatePath))
            {
                throw new InvalidOperationException("Template path not found in the configuration.");
            }

            var templateHtml = await htmlRenderService.RenderHtmlFromTemplatePathAsync(templatePath, entities);

            return templateHtml;
        }
    }
}
