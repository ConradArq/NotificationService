using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.EmailTemplate;

namespace NotificationService.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<ResponseDto<ResponseEmailTemplateDto>> CreateAsync(CreateEmailTemplateDto requestDto);
        Task<ResponseDto<ResponseEmailTemplateDto>> UpdateAsync(int id, UpdateEmailTemplateDto requestDto);
        Task<ResponseDto<object>> DeleteAsync(int id);
        Task<ResponseDto<ResponseEmailTemplateDto>> GetAsync(int id);
        Task<ResponseDto<IEnumerable<ResponseEmailTemplateDto>>> GetAllAsync(RequestDto? requestDto);
        Task<PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>> GetAllPaginatedAsync(PaginationRequestDto requestDto);
        Task<ResponseDto<IEnumerable<ResponseEmailTemplateDto>>> SearchAsync(SearchEmailTemplateDto requestDto);
        Task<PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>> SearchPaginatedAsync(SearchPaginatedEmailTemplateDto requestDto);
    }
}