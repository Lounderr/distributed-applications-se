using WildlifeTracker.Constants;

namespace WildlifeTracker.Exceptions
{
    public class AccessDeniedException : ServiceException
    {
        public AccessDeniedException(string errorDescription) : base(ErrorCodes.AccessDenied, errorDescription)
        {
        }
    }
}
