using System.Security;

namespace CartService.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        public Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            var errorCode = "HB50001";
            var message = "Internal server error";
            var statusCode = StatusCodes.Status500InternalServerError;

            switch (exception)
            {
                case UnauthorizedAccessException:
                    errorCode = "HB40101";
                    message = "Token missing or invalid";
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;
                case ArgumentException or FormatException:
                    errorCode = "HB40001";
                    message = "Missing or invalid input";
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case KeyNotFoundException:
                    errorCode = "HB40401";
                    message = "Resource not found";
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case SecurityException:
                    errorCode = "HB40301";
                    message = "Permission denied";
                    statusCode = StatusCodes.Status403Forbidden;
                    break;
            }

            var response = new
            {
                errorCode,
                message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
