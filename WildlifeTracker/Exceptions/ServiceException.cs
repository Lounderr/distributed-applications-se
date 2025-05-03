using Microsoft.AspNetCore.Identity;

namespace WildlifeTracker.Exceptions
{
    public class ServiceException : ApplicationException
    {
        public readonly IDictionary<string, HashSet<string>> Errors;

        public ServiceException()
            : base("Service exception occurred")
        {
            this.Errors = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        }

        public ServiceException(IDictionary<string, IEnumerable<string>> errors)
            : this()
        {

            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors), "Errors dictionary cannot be null");
            }

            foreach (var error in errors)
            {
                if (string.IsNullOrWhiteSpace(error.Key))
                {
                    throw new ArgumentException("Error code cannot be empty", nameof(errors));
                }

                if (error.Value == null)
                {
                    throw new ArgumentException("Error descriptions cannot be null", nameof(errors));
                }

                if (!this.Errors.TryGetValue(error.Key, out HashSet<string>? errorDescriptions))
                {
                    errorDescriptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    this.Errors[error.Key] = errorDescriptions;
                }

                foreach (var errorDescription in error.Value)
                {
                    if (string.IsNullOrWhiteSpace(errorDescription))
                    {
                        throw new ArgumentException("Error description cannot be null or empty", nameof(errors));
                    }

                    errorDescriptions.Add(errorDescription);
                }
            }
        }

        public ServiceException(string errorCode, string errorDescription)
            : this()
        {
            this.AddError(errorCode, errorDescription);
        }

        public ServiceException(IdentityResult result)
            : this()
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result), $"{nameof(IdentityResult)} object cannot be null");

            if (result.Errors == null)
                throw new ArgumentNullException(nameof(result.Errors), $"{nameof(result.Errors)} object cannot be null");

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    error.Code = "AccountAlreadyExists";
                    error.Description = "An account with this email address already exists.";
                }

                this.AddError(error.Code, error.Description);
            }
        }

        public void AddError(string errorCode, string errorDescription)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentException("Error code cannot be null or empty", nameof(errorCode));
            if (string.IsNullOrWhiteSpace(errorDescription))
                throw new ArgumentException("Error description cannot be null or empty", nameof(errorDescription));

            if (this.Errors.ContainsKey(errorCode))
            {
                this.Errors[errorCode].Add(errorDescription);
            }
            else
            {
                this.Errors[errorCode] = [errorDescription];
            }
        }
    }
}
