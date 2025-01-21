using NotificationService.API.Dtos;

namespace NotificationService.API.Middlewares
{
    /// <summary>
    /// Middleware to handle authorization errors by returning custom JSON responses for 
    /// 401 Unauthorized and 403 Forbidden status codes.
    /// </summary>
    public class AuthErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = ApiResponseDto<object>.Unauthorized();
                await context.Response.WriteAsJsonAsync(response);
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = ApiResponseDto<object>.Forbidden();
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
