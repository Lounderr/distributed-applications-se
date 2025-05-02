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
                await HandleExceptionAsync(context, ex, logger, jsonSerializerOptions);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<CustomExceptionHandlerMiddleware> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            if (context.Response.HasStarted)
                return;

            var response = context.Response;

            response.ContentType = "application/problem+json";

            ValidationProblemDetails problemDetails = exception switch
            {
                BusinessException validationException => HandleValidationException(validationException, response, logger),
                _ => HandleInternalServerError(exception, response, logger)
            };

            if (problemDetails != null)
            {
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
                problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                problemDetails.Instance = context.Request.Path;
            }

            var result = JsonSerializer.Serialize(problemDetails, jsonSerializerOptions);

            await response.WriteAsync(result);

            return;
        }

        private static ValidationProblemDetails HandleValidationException(BusinessException validationException, HttpResponse response, ILogger logger)
        {
            logger.LogWarning(validationException, validationException.ProblemDetails.Title);
            response.StatusCode = validationException.ProblemDetails.Status ?? 400;
            return validationException.ProblemDetails;
        }

        private static ValidationProblemDetails HandleInternalServerError(Exception exception, HttpResponse response, ILogger logger)
        {
            logger.LogError(exception, "Internal server error");
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return new ValidationProblemDetails
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred. Please try again later.",
            };
        }
    }
}
