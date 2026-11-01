namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated.
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "User is not authenticated.")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}
