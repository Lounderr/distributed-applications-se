using WildlifeTracker.Constants;

namespace WildlifeTracker.Exceptions
{
    public class UnauthorizedException : BusinessException
    {
        public UnauthorizedException(string errorDescription) : base(ErrorCodes.Unauthorized, errorDescription)
            => this.ProblemDetails.Status = 401;
    }
}
