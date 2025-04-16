using Microsoft.AspNetCore.Mvc;
using System.Net;
using NotificationService.API.Dtos;
using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.Notification.Push;
using NotificationService.Domain.Models.Entities;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Dtos.Reports;
using Microsoft.AspNetCore.Authorization;
using NotificationService.API.Filters;
using NotificationService.Domain.Enums;
using NotificationService.Application.Dtos.Notification.Email;

namespace NotificationService.API.Controllers
{
    [Authorize(Roles = "Admin, Manager, Supervisor")]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
    public class PushNotificationController : Controller
    {
        private INotificationService<PushNotification, ResponsePushNotificationDto> _notificationService;
        private IReportService<PushNotification> _reportService;

        public PushNotificationController(INotificationService<PushNotification, ResponsePushNotificationDto> notificationService, IReportService<PushNotification> reportService)
        {
            _notificationService = notificationService;
            _reportService = reportService;
        }

        [HttpPost("send")]
        [Authorize(Policy = "SendNotificationPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailNotificationDto>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Send([FromForm] CreatePushNotificationDto requestDto)
        {
            var responseDto = await _notificationService.NotifyUsersAsync(requestDto);
            var apiResponseDto = ApiResponseDto<ResponsePushNotificationDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPut("update/{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ValidateEntityStatus(typeof(PushNotification), new []{ Status.Active})]
        [ProducesResponseType(typeof(ApiResponseDto<ResponsePushNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdatePushNotificationDto requestDto)
        {
            var responseDto = await _notificationService.UpdateAsync(id, requestDto);
            var apiResponseDto = ApiResponseDto<ResponsePushNotificationDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponsePushNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get([FromRoute] int id)
        {
            var responseDto = await _notificationService.GetAsync(id);
            var apiResponseDto = ApiResponseDto<ResponsePushNotificationDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponsePushNotificationDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll([FromQuery] RequestDto? requestDto)
        {
            var responseDto = await _notificationService.GetAllAsync(requestDto);
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponsePushNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("paginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponsePushNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllPaginated([FromBody] PaginationRequestDto requestDto)
        {
            var responseDto = await _notificationService.GetAllPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponsePushNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("search")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponsePushNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Search([FromBody] SearchPushNotificationDto requestDto)
        {
            var responseDto = await _notificationService.SearchAsync(requestDto);
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponsePushNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("searchpaginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponsePushNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SearchPaginated([FromBody] SearchPaginatedPushNotificationDto requestDto)
        {
            var responseDto = await _notificationService.SearchPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponsePushNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet("generatereport")]
        [Authorize(Roles = "Admin, Manager", Policy = "GenerateNotificationsReportPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseReportDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GenerateReport()
        {
            var responseDto = await _reportService.GenerateReportAsync();
            var apiResponseDto = ApiResponseDto<ResponseReportDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }
    }
}
