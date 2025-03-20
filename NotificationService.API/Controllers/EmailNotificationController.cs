using Microsoft.AspNetCore.Mvc;
using System.Net;
using NotificationService.API.Dtos;
using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Domain.Models.Entities;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Dtos.Reports;
using Microsoft.AspNetCore.Authorization;
using NotificationService.Shared.Resources;

namespace NotificationService.API.Controllers
{
    [Authorize (Roles ="Admin, Manager, Supervisor")]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
    public class EmailNotificationController : Controller
    {
        private INotificationService<EmailNotification, ResponseEmailNotificationDto> _notificationService;
        private IReportService<EmailNotification> _reportService;

        public EmailNotificationController(INotificationService<EmailNotification, ResponseEmailNotificationDto> notificationService, IReportService<EmailNotification> reportService)
        {
            _notificationService = notificationService;
            _reportService = reportService;
        }

        [HttpPost("send")]
        [Authorize(Policy = "SendNotificationPolicy")]
        // Emails are sent asynchronously in a background service. Success or failure is communicated to the client through SignalR.
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailNotificationDto>), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Send([FromForm] CreateEmailNotificationDto requestDto)
        {
            var responseDto = await _notificationService.NotifyUsersAsync(requestDto);
            responseDto.Message = GeneralMessages.EmailSendingProcessStartedMessage;
            var apiResponseDto = ApiResponseDto<ResponseEmailNotificationDto>.Accepted(responseDto!);
            return Accepted(apiResponseDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get([FromRoute] int id)
        {
            var responseDto = await _notificationService.GetAsync(id);
            var apiResponseDto = ApiResponseDto<ResponseEmailNotificationDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponseEmailNotificationDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            var responseDto = await _notificationService.GetAllAsync();
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponseEmailNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("paginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponseEmailNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllPaginated([FromBody] PaginationRequestDto requestDto)
        {
            var responseDto = await _notificationService.GetAllPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponseEmailNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("search")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponseEmailNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Search([FromBody] SearchEmailNotificationDto requestDto)
        {
            var responseDto = await _notificationService.SearchAsync(requestDto);
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponseEmailNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("searchpaginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponseEmailNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SearchPaginated([FromBody] SearchPaginatedEmailNotificationDto requestDto)
        {
            var responseDto = await _notificationService.SearchPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponseEmailNotificationDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet("generatereport")]
        [Authorize(Roles = "Admin, Manager", Policy = "GenerateNotificationsReportPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailNotificationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GenerateReport()
        {
            var responseDto = await _reportService.GenerateReportAsync();
            var apiResponseDto = ApiResponseDto<ResponseReportDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }
    }
}
