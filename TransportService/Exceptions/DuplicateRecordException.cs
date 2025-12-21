namespace TransportService.Exceptions
{
    public class DuplicateRecordException : Exception
    {
        public string ErrorCode { get; }

        public DuplicateRecordException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
