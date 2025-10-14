using System;
using System.Net;
using System.Text.Json;

namespace Warehouse_ERPIntegration.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred while processing {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            // Map exception types or status codes
            switch (ex)
            {
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "You are not authorized to perform this action.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = "The requested resource was not found.";
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = (int)statusCode,
                error = message,
                details = ex.Message // optional: include only for internal logs, not for clients in production
            });

            await context.Response.WriteAsync(result);
        }
    }
}
