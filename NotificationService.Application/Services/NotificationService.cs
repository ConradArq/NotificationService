using AutoMapper;
using NotificationService.Application.Dtos;
using NotificationService.Application.Exceptions;
using NotificationService.Shared.Helpers;
using NotificationService.Application.Dtos.Notification;
using NotificationService.Domain.Models;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services
{
    public class NotificationService<TEntity, TResponse> : INotificationService<TEntity, TResponse> where TEntity : Notification
    {
        private readonly IMapper _mapper;
        private readonly INotificationHandlerFactory _notificationHandlerFactory;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IMapper mapper, INotificationHandlerFactory notificationHandlerFactory, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _notificationHandlerFactory = notificationHandlerFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<IEnumerable<TResponse>>> GetAllAsync()
        {
            var entities = await _unitOfWork.Repository<TEntity>().GetAsync();
            IEnumerable<TResponse> responseNotificationDtos = _mapper.Map<IEnumerable<TResponse>>(entities);
            var response = new ResponseDto<IEnumerable<TResponse>>(responseNotificationDtos);
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<TResponse>>> GetAllPaginatedAsync(PaginationRequestDto requestDto)
        {
            var entities = await _unitOfWork.Repository<TEntity>().GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize);
            var response = new PaginatedResponseDto<IEnumerable<TResponse>>(_mapper.Map<IEnumerable<TResponse>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }

        public async Task<ResponseDto<TResponse>> GetAsync(int id)
        {
            var entity = await _unitOfWork.Repository<TEntity>().GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            var response = new ResponseDto<TResponse>(_mapper.Map<TResponse>(entity));
            return response;
        }

        public async Task<ResponseDto<TResponse>> NotifyUsersAsync(CreateNotificationDto requestDto)
        {
            var handler = (dynamic)_notificationHandlerFactory.GetHandler(requestDto.GetType());
            var notification = await handler.HandleAsync((dynamic)requestDto);

            var entity = _unitOfWork.Repository<TEntity>().Create(notification);
            await _unitOfWork.SaveAsync();

            return new ResponseDto<TResponse>(_mapper.Map<TResponse>(entity));
        }

        public async Task<ResponseDto<TResponse>> UpdateAsync(int id, UpdateNotificationDto requestDto)
        {
            var entity = await _unitOfWork.Repository<TEntity>().GetSingleAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(id);
            }

            _mapper.Map(requestDto, entity);
            await _unitOfWork.SaveAsync();

            var response = new ResponseDto<TResponse>(_mapper.Map<TResponse>(entity));
            return response;
        }

        public async Task<ResponseDto<IEnumerable<TResponse>>> SearchAsync(SearchNotificationDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<TEntity>(requestDto);
            var entities = await _unitOfWork.Repository<TEntity>().GetAsync(searchExpression);
            var response = new ResponseDto<IEnumerable<TResponse>>(_mapper.Map<IEnumerable<TResponse>>(entities));
            return response;
        }

        public async Task<PaginatedResponseDto<IEnumerable<TResponse>>> SearchPaginatedAsync(SearchPaginatedNotificationDto requestDto)
        {
            var searchExpression = QueryHelper.BuildPredicate<TEntity>(requestDto);
            var entities = await _unitOfWork.Repository<TEntity>().GetPaginatedAsync(requestDto.PageNumber, requestDto.PageSize, searchExpression);
            var response = new PaginatedResponseDto<IEnumerable<TResponse>>(_mapper.Map<IEnumerable<TResponse>>(entities.Data), requestDto.PageNumber, requestDto.PageSize, entities.TotalItems);
            return response;
        }
    }
}
