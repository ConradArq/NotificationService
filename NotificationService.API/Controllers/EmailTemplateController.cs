using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Dtos;
using NotificationService.Application.Dtos;
using NotificationService.Application.Dtos.EmailTemplate;
using NotificationService.Application.Dtos.SmtpConfig;
using NotificationService.Application.Exceptions;
using NotificationService.Application.Interfaces.Services;
using System.Net;

namespace NotificationService.API.Controllers
{
    // Only authentication is required; no specific authorization policies or roles are applied.
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
    public class EmailTemplateController : Controller
    {
        private IEmailTemplateService _emailTemplateService;

        public EmailTemplateController(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateEmailTemplateDto requestDto)
        {
            var responseDto = await _emailTemplateService.CreateAsync(requestDto);
            var apiResponseDto = ApiResponseDto<ResponseEmailTemplateDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPut("update/{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateEmailTemplateDto requestDto)
        {
            var responseDto = await _emailTemplateService.UpdateAsync(id, requestDto);
            var apiResponseDto = ApiResponseDto<ResponseEmailTemplateDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var responseDto = await _emailTemplateService.DeleteAsync(id);
            var apiResponseDto = ApiResponseDto<object>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "EntityOwnershipPolicy")]
        [ProducesResponseType(typeof(ApiResponseDto<ResponseEmailTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get([FromRoute] int id)
        {
            var responseDto = await _emailTemplateService.GetAsync(id);
            var apiResponseDto = ApiResponseDto<ResponseEmailTemplateDto>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponseEmailTemplateDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll([FromQuery] RequestDto? requestDto)
        {
            var responseDto = await _emailTemplateService.GetAllAsync(requestDto);
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponseEmailTemplateDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("paginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllPaginated([FromBody] PaginationRequestDto requestDto)
        {
            var responseDto = await _emailTemplateService.GetAllPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("search")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<ResponseEmailTemplateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Search([FromBody] SearchEmailTemplateDto requestDto)
        {
            var responseDto = await _emailTemplateService.SearchAsync(requestDto);
            var apiResponseDto = ApiResponseDto<IEnumerable<ResponseEmailTemplateDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }

        [HttpPost("searchpaginated")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ApiPaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SearchPaginated([FromBody] SearchPaginatedEmailTemplateDto requestDto)
        {
            var responseDto = await _emailTemplateService.SearchPaginatedAsync(requestDto);
            var apiResponseDto = ApiPaginatedResponseDto<IEnumerable<ResponseEmailTemplateDto>>.Ok(responseDto!);
            return Ok(apiResponseDto);
        }
    }
}
