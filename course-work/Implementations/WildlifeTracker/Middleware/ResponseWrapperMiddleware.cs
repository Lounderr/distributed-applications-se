using System.Text;
using System.Text.Json;

namespace WildlifeTracker.Middleware
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ResponseWrapperMiddleware(RequestDelegate next, JsonSerializerOptions jsonSerializerOptions)
        {
            this._next = next;
            this._jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            // Capture the original response body
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await this._next(context);

                // Only wrap successful JSON responses
                if (this.PathIsNotExcluded(context.Request.Path.Value) &&
                    context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 &&
                    context.Response.ContentType != null &&
                    context.Response.ContentType.Contains("application/json"))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var originalResponse = await new StreamReader(memoryStream).ReadToEndAsync();

                    var wrapped = JsonSerializer.Serialize(new
                    {
                        status = context.Response.StatusCode,
                        data = JsonSerializer.Deserialize<object>(originalResponse, this._jsonSerializerOptions)
                    }, this._jsonSerializerOptions);

                    context.Response.ContentLength = Encoding.UTF8.GetByteCount(wrapped);
                    context.Response.Body = originalBodyStream;
                    await context.Response.WriteAsync(wrapped);
                }
                else
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool PathIsNotExcluded(string? requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                return true;

            string[] excludedPaths = { "/swagger", "/api/v1/identity/login" };

            foreach (var path in excludedPaths)
            {
                if (requestPath.StartsWith(path))
                    return false;
            }

            return true;
        }
    }
}
