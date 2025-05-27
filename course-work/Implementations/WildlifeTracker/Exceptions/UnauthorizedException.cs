using WildlifeTracker.Constants;

namespace WildlifeTracker.Exceptions
{
    public class UnauthorizedException : ServiceException
    {
        public UnauthorizedException(string errorDescription) : base(ErrorCodes.Unauthorized, errorDescription)
        {
        }
    }
}
