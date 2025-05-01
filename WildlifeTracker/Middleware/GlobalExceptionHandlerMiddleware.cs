using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace WildlifeTracker.Middleware
{
    public sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            var response = context.Response;

            response.ContentType = "application/json";

            if (exception is ValidationException)
            {
                logger.LogWarning(exception, "Bad request");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                logger.LogError(exception, "Internal server error");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }


            var result = JsonSerializer.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                message = exception is ValidationException ? exception.Message : "Internal server error",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                traceId = Activity.Current?.Id ?? context.TraceIdentifier,
            });

            return response.WriteAsync(result);
        }
    }
}
