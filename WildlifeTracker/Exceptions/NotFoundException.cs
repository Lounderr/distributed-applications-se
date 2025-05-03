using WildlifeTracker.Constants;

namespace WildlifeTracker.Exceptions
{
    public class NotFoundException : ServiceException
    {
        public NotFoundException(string errorDescription) : base(ErrorCodes.EntityNotFound, errorDescription)
        {
        }
    }
}
