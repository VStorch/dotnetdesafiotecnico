using Microsoft.AspNetCore.Diagnostics;
using TaskManager.Domain.Exceptions;
using TaskManager.WebApi.Responses;

namespace TaskManager.WebApi.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            var errorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred on the server."
            };

            if (exception is EmailAlreadyExistsException emailException)
            {
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = emailException.Message;
            }

            httpContext.Response.StatusCode = errorResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

            return true;
        }
    }
}