using Microsoft.AspNetCore.Diagnostics;

namespace TaskManagementSystem.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception has occurred while executing the request.");

            httpContext.Response.StatusCode = 500;
            string[] errors = ["Oops.. Something went wrong."];
            await httpContext.Response.WriteAsJsonAsync(new { Errors = errors }, cancellationToken: cancellationToken);

            return true;
        }
    }
}
