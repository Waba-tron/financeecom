namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Base exception for all application-level errors.
/// </summary>
public class AppException : Exception
{
    public string? ErrorCode { get; set; }
    public int? StatusCode { get; set; }

    public AppException(string message, string? errorCode = null, int? statusCode = 400)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
