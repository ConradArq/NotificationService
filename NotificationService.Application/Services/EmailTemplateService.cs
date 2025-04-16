using AutoMapper;
using AutoMapper.QueryableExtensions;
using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.EmailTemplate;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Helpers;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;

namespace NotificationService.Application.Services
{
    internal class EmailTemplateService : IEmailTemplateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<ResponseEmailTemplateDto>> CreateAsync(CreateEmailTemplateDto requestDto)
        {
            var entity = _mapper.Map<EmailTemplate>(requestDto);

            _unitOfWork.EmailTemplateRepository.Create(entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<ResponseEmailTemplateDto>(_mapper.Map<ResponseEmailTemplateDto>(entity));
            return response;
        }

        public async Task<ResponseDto<ResponseEmailTemplateDto>> UpdateAsync(int id, UpdateEmailTemplateDto requestDto)
        {
            var entity = await _unitOfWork.EmailTemplateRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            _mapper.Map(requestDto, entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<ResponseEmailTemplateDto>(_mapper.Map<ResponseEmailTemplateDto>(entity));
            return response;
        }

        public async Task<ResponseDto<object>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.EmailTemplateRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            _unitOfWork.EmailTemplateRepository.Delete(entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<object>();
            return response;
        }

        public async Task<ResponseDto<ResponseEmailTemplateDto>> GetAsync(int id)
        {
            var entity = await _unitOfWork.EmailTemplateRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            var response = new ResponseDto<ResponseEmailTemplateDto>(_mapper.Map<ResponseEmailTemplateDto>(entity));
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ResponseEmailTemplateDto>>> GetAllAsync(RequestDto? requestDto)
        {
            var selector = new Func<IQueryable<EmailTemplate>, IQueryable<ResponseEmailTemplateDto>>(query => query
                 .ProjectTo<ResponseEmailTemplateDto>(_mapper.ConfigurationProvider)
             );

            var responseDtos = await _unitOfWork.EmailTemplateRepository.GetAsync(
                orderBy: QueryHelper.BuildOrderByFunction<EmailTemplate>(requestDto),
                selector: selector
            );

            var response = new ResponseDto<IEnumerable<ResponseEmailTemplateDto>>(responseDtos);
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>> GetAllPaginatedAsync(PaginationRequestDto requestDto)
        {
            var entities = await _unitOfWork.EmailTemplateRepository.GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize, orderBy: QueryHelper.BuildOrderByFunction<EmailTemplate>(requestDto));

            var response = new PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>(_mapper.Map<IEnumerable<ResponseEmailTemplateDto>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ResponseEmailTemplateDto>>> SearchAsync(SearchEmailTemplateDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<EmailTemplate>(requestDto);
            var entities = await _unitOfWork.EmailTemplateRepository.GetAsync(searchExpression, orderBy: QueryHelper.BuildOrderByFunction<EmailTemplate>(requestDto));

            var response = new ResponseDto<IEnumerable<ResponseEmailTemplateDto>>(_mapper.Map<IEnumerable<ResponseEmailTemplateDto>>(entities));
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>> SearchPaginatedAsync(SearchPaginatedEmailTemplateDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<EmailTemplate>(requestDto);
            var entities = await _unitOfWork.EmailTemplateRepository.GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize, searchExpression, orderBy: QueryHelper.BuildOrderByFunction<EmailTemplate>(requestDto));

            var response = new PaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>(_mapper.Map<IEnumerable<ResponseEmailTemplateDto>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }
    }
}
