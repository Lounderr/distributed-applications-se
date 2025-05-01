namespace WildlifeTracker.Exceptions
{
    public class ValidationException : SystemException
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ValidationException(string message, string fieldName) : base(message)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; } = string.Empty;
    }
}
