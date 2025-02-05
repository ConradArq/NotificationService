using AutoMapper;
using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.SmtpConfig;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;
using NotificationService.Shared.Helpers;

namespace NotificationService.Application.Services
{
    internal class SmtpConfigService : ISmtpConfigService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SmtpConfigService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<ResponseSmtpConfigDto>> CreateAsync(CreateSmtpConfigDto requestDto)
        {
            var entity = _mapper.Map<SmtpConfig>(requestDto);

            _unitOfWork.SmtpConfigRepository.Create(entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<ResponseSmtpConfigDto>(_mapper.Map<ResponseSmtpConfigDto>(entity));
            return response;
        }

        public async Task<ResponseDto<ResponseSmtpConfigDto>> UpdateAsync(int id, UpdateSmtpConfigDto requestDto)
        {
            var entity = await _unitOfWork.SmtpConfigRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            _mapper.Map(requestDto, entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<ResponseSmtpConfigDto>(_mapper.Map<ResponseSmtpConfigDto>(entity));
            return response;
        }

        public async Task<ResponseDto<object>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SmtpConfigRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            _unitOfWork.SmtpConfigRepository.Delete(entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<object>();
            return response;
        }

        public async Task<ResponseDto<ResponseSmtpConfigDto>> GetAsync(int id)
        {
            var entity = await _unitOfWork.SmtpConfigRepository.GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            var response = new ResponseDto<ResponseSmtpConfigDto>(_mapper.Map<ResponseSmtpConfigDto>(entity));
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ResponseSmtpConfigDto>>> GetAllAsync()
        {
            var entities = await _unitOfWork.SmtpConfigRepository.GetAsync();

            var response = new ResponseDto<IEnumerable<ResponseSmtpConfigDto>>(_mapper.Map<IEnumerable<ResponseSmtpConfigDto>>(entities));
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>> GetAllPaginatedAsync(PaginationRequestDto requestDto)
        {
            var entities = await _unitOfWork.SmtpConfigRepository.GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize);

            var response = new PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>(_mapper.Map<IEnumerable<ResponseSmtpConfigDto>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ResponseSmtpConfigDto>>> SearchAsync(SearchSmtpConfigDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<SmtpConfig>(requestDto);
            var entities = await _unitOfWork.SmtpConfigRepository.GetAsync(searchExpression);

            var response = new ResponseDto<IEnumerable<ResponseSmtpConfigDto>>(_mapper.Map<IEnumerable<ResponseSmtpConfigDto>>(entities));
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>> SearchPaginatedAsync(SearchPaginatedSmtpConfigDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<SmtpConfig>(requestDto);
            var entities = await _unitOfWork.SmtpConfigRepository.GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize, searchExpression);

            var response = new PaginatedResponseDto<IEnumerable<ResponseSmtpConfigDto>>(_mapper.Map<IEnumerable<ResponseSmtpConfigDto>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }
    }
}