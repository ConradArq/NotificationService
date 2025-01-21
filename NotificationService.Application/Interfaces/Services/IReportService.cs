using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.Reports;

namespace NotificationService.Application.Interfaces.Services
{
    public interface IReportService<TModel>
    {
        Task<ResponseDto<ResponseReportDto>> GenerateReportAsync();       
    }
}