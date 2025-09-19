namespace BankMore.Transferencia.Services;

public sealed class WorkException : Exception
{
    public string ErrorType { get; }

    public WorkException(string errorType, string message)
        : base(message)
    {
        ErrorType = errorType;
    }
}