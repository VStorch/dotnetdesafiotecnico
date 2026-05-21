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

            var (statusCode, message) = exception switch
            {
                EmailAlreadyExistsException e => (StatusCodes.Status400BadRequest, e.Message),
                InvalidCredentialsException e => (StatusCodes.Status401Unauthorized, e.Message),
                TaskNotFoundException e => (StatusCodes.Status404NotFound, e.Message),

                DomainException e => (StatusCodes.Status400BadRequest, e.Message),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message
            }, cancellationToken);

            return true;
        }
    }
}