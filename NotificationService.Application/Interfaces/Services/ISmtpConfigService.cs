using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.SmtpConfig;

namespace NotificationService.Application.Interfaces.Services
{
    public interface ISmtpConfigService
    {
        Task<ResponseDto<ResponseSmtpConfigDto>> CreateAsync(CreateSmtpConfigDto requestDto);
        Task<ResponseDto<ResponseSmtpConfigDto>> UpdateAsync(int id, UpdateSmtpConfigDto requestDto);
        Task<ResponseDto<object>> DeleteAsync(int id);
        Task<ResponseDto<ResponseSmtpConfigDto>> GetAsync(int id);
        Task<ResponseDto<IEnumerable<ResponseSmtpConfigDto>>> GetAllAsync();
        Task<PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>> GetAllPaginatedAsync(PaginationRequestDto requestDto);
        Task<ResponseDto<IEnumerable<ResponseSmtpConfigDto>>> SearchAsync(SearchSmtpConfigDto requestDto);
        Task<PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>> SearchPaginatedAsync(SearchPaginatedSmtpConfigDto requestDto);
    }
}
