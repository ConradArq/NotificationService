using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.Notification;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Interfaces.Services
{
    public interface INotificationService<TEntity, TResponse> where TEntity : BaseDomainModel
    {
        Task<ResponseDto<TResponse>> NotifyUsersAsync(CreateNotificationDto createNotificationDto);
        Task<ResponseDto<TResponse>> GetAsync(int id);
        Task<ResponseDto<IEnumerable<TResponse>>> GetAllAsync();
        Task<PaginatedResponseDto<IEnumerable<TResponse>>> GetAllPaginatedAsync(PaginationRequestDto requestDto);
        Task<ResponseDto<IEnumerable<TResponse>>> SearchAsync(SearchNotificationDto requestDto);
        Task<PaginatedResponseDto<IEnumerable<TResponse>>> SearchPaginatedAsync(SearchPaginatedNotificationDto requestDto);
        Task<ResponseDto<TResponse>> UpdateAsync(int id, UpdateNotificationDto requestDto);
    }
}