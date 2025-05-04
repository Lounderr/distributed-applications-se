#pragma warning disable CS8618 
namespace WildlifeTracker.Constants
{
    public static class ErrorCodes
    {
        static ErrorCodes()
        {
            // Cycle through the static properties and assign a value to them which is equal to nameof(property)
            foreach (var prop in typeof(ErrorCodes).GetProperties())
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(null, prop.Name);
                }
            }
        }

        public static string EmailInvalid { get; internal set; }
        public static string GetParametersInvalid { get; internal set; }
        public static string MobileInvalid { get; internal set; }
        public static string InvalidPageSize { get; internal set; }
        public static string InvalidPageNumber { get; internal set; }
        public static string EntityNotFound { get; internal set; }
        public static string Unauthorized { get; internal set; }
        public static string IdMismatch { get; internal set; }
    }
}
#pragma warning restore CS8618
