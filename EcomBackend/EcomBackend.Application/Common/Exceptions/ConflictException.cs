namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a resource already exists (409 Conflict).
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message, string? errorCode = null)
        : base(message, errorCode ?? "CONFLICT", 409)
    {
    }
}
