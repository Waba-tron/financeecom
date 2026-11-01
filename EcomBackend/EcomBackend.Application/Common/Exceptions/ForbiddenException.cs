namespace EcomBackend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user lacks required permissions.
/// </summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "User does not have permission to access this resource.")
        : base(message, "FORBIDDEN", 403)
    {
    }
}
