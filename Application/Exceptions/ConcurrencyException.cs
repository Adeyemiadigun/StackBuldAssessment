using Application.Exceptions;

public class ConcurrencyException : ApiException
{
    private const int Status = 409;
    private const string Code = "ConcurrencyConflict";

    public ConcurrencyException(string message)
        : base(message, Status, Code)
    {
    }
}

