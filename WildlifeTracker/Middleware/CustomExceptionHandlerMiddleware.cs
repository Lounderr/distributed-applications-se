using System.Diagnostics;
using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Middleware
{
    public sealed class CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, ILogger<CustomExceptionHandlerMiddleware> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                using (logger.BeginScope("CustomExceptionHandlerMiddleware"))
                {
                    await HandleExceptionAsync(context, ex, logger, jsonSerializerOptions);
                }
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<CustomExceptionHandlerMiddleware> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            if (context.Response.HasStarted)
                return;

            var response = context.Response;
            response.ContentType = "application/problem+json";

            ValidationProblemDetails problemDetails = new();

            if (exception is ServiceException serviceException)
            {
                logger.LogWarning(serviceException, "Service exception occured");

                problemDetails.Title = "One or more business errors have occurred.";
                problemDetails.Type = "business-error";

                problemDetails.Status = serviceException switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    AccessDeniedException => (int)HttpStatusCode.Forbidden,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.UnprocessableEntity
                };

                problemDetails.Errors = serviceException.Errors.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToArray()
                );
            }
            else
            {
                logger.LogError(exception, "Internal server error occured");

                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Type = "internal-server-error";
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
            }

            problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            problemDetails.Instance = context.Request.Path;

            var result = JsonSerializer.Serialize(problemDetails, jsonSerializerOptions);

            await response.WriteAsync(result);

            return;
        }
    }
}
