using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WildlifeTracker.Exceptions
{
    public class BusinessException : ApplicationException
    {
        public ValidationProblemDetails ProblemDetails { get; }

        public BusinessException(IDictionary<string, string[]> errorsDictionary)
            : base($"Field validation error occurred") =>
            this.ProblemDetails = CreateValidationProblem(errorsDictionary);

        public BusinessException(IdentityResult result)
            : base("Identity validation error occurred") =>
            this.ProblemDetails = CreateValidationProblem(result);

        public BusinessException(string errorCode, string errorDescription)
            : base($"General business error: {errorCode} - {errorDescription}") =>
            this.ProblemDetails = CreateGeneralBusinessError(errorCode, errorDescription);

        public void AddError(string errorCode, string errorDescription)
        {

            if (this.ProblemDetails == null)
                throw new ArgumentException("ProblemDetails not initialized", nameof(errorCode));
            if (this.ProblemDetails.Errors == null)
                this.ProblemDetails.Errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentException("Error code cannot be null or empty", nameof(errorCode));
            if (string.IsNullOrWhiteSpace(errorDescription))
                throw new ArgumentException("Error description cannot be null or empty", nameof(errorDescription));
            if (this.ProblemDetails.Errors.ContainsKey(errorCode))
            {
                // Deduplication of error descriptions
                var descriptions = this.ProblemDetails.Errors[errorCode].ToHashSet();
                descriptions.Add(errorDescription);
                this.ProblemDetails.Errors[errorCode] = descriptions.ToArray();
            }
            else
                this.ProblemDetails.Errors.Add(errorCode, new[] { errorDescription });
        }

        private static ValidationProblemDetails CreateValidationProblem(IDictionary<string, string[]> errorsDictionary)
        {
            if (errorsDictionary == null)
                throw new ArgumentException($"Parameter {nameof(errorsDictionary)} cannot be null", nameof(errorsDictionary));

            foreach (var entry in errorsDictionary)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value == null || entry.Value.Any(string.IsNullOrWhiteSpace))
                    throw new ArgumentException($"Parameter {nameof(errorsDictionary)} contains invalid entries", nameof(errorsDictionary));
            }

            return new ValidationProblemDetails(errorsDictionary)
            {
                Title = "Validation Error",
                Status = 422,
                Detail = "One or more validation errors occurred",
            };
        }

        private static ValidationProblemDetails CreateValidationProblem(IdentityResult result)
        {
            if (result == null)
                throw new ArgumentException($"{nameof(IdentityResult)} object cannot be null", nameof(result));

            if (result.Errors == null)
                throw new ArgumentException($"{nameof(result.Errors)} object cannot be null", nameof(result));

            var errorDictionary = new Dictionary<string, string[]>();

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    error.Code = "AccountAlreadyExists";
                    error.Description = "An account with this email address already exists.";
                }

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    errorDictionary[error.Code] = descriptions.Append(error.Description).ToArray();
                }
                else
                {
                    errorDictionary[error.Code] = [error.Description];
                }
            }

            return CreateValidationProblem(errorDictionary);
        }

        private static ValidationProblemDetails CreateGeneralBusinessError(string errorCode, string errorDescription)
        {
            return CreateValidationProblem(new Dictionary<string, string[]> { { errorCode, [errorDescription] } });
        }
    }
}
