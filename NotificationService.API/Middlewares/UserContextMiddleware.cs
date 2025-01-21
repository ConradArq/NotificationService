using NotificationService.Infrastructure.Persistence;
using System.Security.Claims;

namespace NotificationService.API.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Middleware responsible for setting the current user's ID in the DbContext.
        /// This ID is automatically assigned to the CreatedBy and LastModifiedBy properties of each entity during data operations.
        /// </summary>
        public UserContextMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, NotificationServiceDbContext dbContext)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            dbContext.CurrentUserId = userId;

            await _next(context);
        }
    }
}
