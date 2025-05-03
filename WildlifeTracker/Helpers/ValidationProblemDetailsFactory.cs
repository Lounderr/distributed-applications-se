using System.Diagnostics;
using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace WildlifeTracker.Helpers
{
    public static class ValidationProblemDetailsFactory
    {
        public static BadRequestObjectResult CreateInvalidModelStateResponse(ActionContext actionContext)
        {
            if (actionContext?.ModelState == null)
            {
                throw new ArgumentNullException(nameof(actionContext.ModelState), "ModelState cannot be null.");
            }

            var errors = actionContext.ModelState
                .Where(ms => ms.Value?.Errors?.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                );

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors have occurred.",
                Status = (int)HttpStatusCode.BadRequest,
                Type = "validation-error"
            };

            problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            problemDetails.Instance = actionContext.HttpContext.Request.Path;

            return new BadRequestObjectResult(problemDetails);
        }
    }
}
