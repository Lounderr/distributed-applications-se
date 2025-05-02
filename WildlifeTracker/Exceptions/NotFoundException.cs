using WildlifeTracker.Constants;

namespace WildlifeTracker.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException(string errorDescription) : base(ErrorCodes.EntityNotFound, errorDescription)
            => this.ProblemDetails.Status = 404;
    }
}
